using System;
using UnityEngine;

namespace Decay.Server.ConsoleApp {
  public class ConsoleInput {
    public string inputString = "";
    public string[] statusText = new string[3] {"", "", ""};
    internal float nextUpdate;

    public event Action<string> OnInputText;

    public bool valid => Console.BufferWidth > 0;

    public int lineWidth => Console.BufferWidth;

    public void ClearLine(int numLines) {
      Console.CursorLeft = 0;
      Console.Write(new string(' ', lineWidth * numLines));
      Console.CursorTop -= numLines;
      Console.CursorLeft = 0;
    }

    public void RedrawInputLine() {
      var backgroundColor = Console.BackgroundColor;
      var foregroundColor = Console.ForegroundColor;
      try {
        Console.ForegroundColor = ConsoleColor.White;
        ++Console.CursorTop;
        for ( var index = 0; index < statusText.Length; ++index ) {
          Console.CursorLeft = 0;
          Console.Write(statusText[index].PadRight(lineWidth));
        }

        Console.CursorTop -= statusText.Length + 1;
        Console.CursorLeft = 0;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Green;
        ClearLine(1);
        if ( inputString.Length == 0 ) {
          Console.BackgroundColor = backgroundColor;
          Console.ForegroundColor = foregroundColor;
          return;
        }

        if ( inputString.Length < lineWidth - 2 )
          Console.Write(inputString);
        else
          Console.Write(inputString.Substring(inputString.Length - (lineWidth - 2)));
      }
      catch ( Exception ex ) {
        // ignored
      }

      Console.BackgroundColor = backgroundColor;
      Console.ForegroundColor = foregroundColor;
    }

    internal void OnBackspace() {
      if ( inputString.Length < 1 )
        return;
      inputString = inputString.Substring(0, inputString.Length - 1);
      RedrawInputLine();
    }

    internal void OnEscape() {
      inputString = "";
      RedrawInputLine();
    }

    internal void OnEnter() {
      ClearLine(statusText.Length);
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("> " + this.inputString);
      var inputString = this.inputString;
      this.inputString = "";
      if ( OnInputText != null )
        OnInputText(inputString);
      RedrawInputLine();
    }

    public void Update() {
      if ( !valid )
        return;
      if ( nextUpdate < (double) Time.realtimeSinceStartup ) {
        RedrawInputLine();
        nextUpdate = Time.realtimeSinceStartup + 0.5f;
      }

      try {
        if ( !Console.KeyAvailable )
          return;
      }
      catch ( Exception ex ) {
        return;
      }

      var consoleKeyInfo = Console.ReadKey();
      if ( consoleKeyInfo.Key == ConsoleKey.Enter )
        OnEnter();
      else if ( consoleKeyInfo.Key == ConsoleKey.Backspace )
        OnBackspace();
      else if ( consoleKeyInfo.Key == ConsoleKey.Escape ) {
        OnEscape();
      } else {
        if ( consoleKeyInfo.KeyChar == char.MinValue )
          return;
        inputString += consoleKeyInfo.KeyChar.ToString();
        RedrawInputLine();
      }
    }
  }
}