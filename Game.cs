using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D
{
  int[,] matrix;
  int[,] next;
  TileMapLayer grid;
  Camera2D cam;
  Timer timer;
  Button startButton;
  Button stopButton;
  HSlider speedSlider;
  Vector2 move;
  Vector2 oldCamPos;

  public override void _Ready(){
    grid = (TileMapLayer)GetNode("TileMapLayer");
    cam = (Camera2D)GetNode("Camera2D");
    timer = (Timer)GetNode("Timer");
    timer.Timeout += OnTimerTimeout;
    startButton = (Button)GetNode("CanvasLayer/Buttons/StartButton");
    startButton.Pressed += OnStartButtonPressed;
    stopButton = (Button)GetNode("CanvasLayer/Buttons/StopButton");
    stopButton.Pressed += OnStopButtonPressed;
    speedSlider = (HSlider)GetNode("CanvasLayer/SpeedSlider");
    speedSlider.ValueChanged += OnSpeedSliderValueChanged;
    matrix = new int[100, 100];
    next = new int[100, 100];
  }
  
  public void SetCell(int i, int j, int TileId){
    this.grid.SetCell(new Vector2I(i, j), 0, new Vector2I(TileId, 0));
  }

  public void DrawMatrix(){
    /*Vector2 viewportSize = cam.GetViewportRect().Size/32*cam.Zoom.Length();
    Vector2 cameraPos = cam.GlobalPosition/32*cam.Zoom.Length();
    for(int i = (int)(cameraPos.X - viewportSize.X/2); i < Math.Ceiling(cameraPos.X + viewportSize.X/2); i++){
      for(int j = (int)(cameraPos.Y - viewportSize.Y/2); j < Math.Ceiling(cameraPos.Y + viewportSize.Y/2); j++){
        SetCell(i, j, this.matrix[i, j]);
      }
    }*/
    for(int i = 0; i < matrix.GetLength(0); i++){
      for(int j = 0; j < matrix.GetLength(1); j++){
        SetCell(i, j, matrix[i, j]);
      }
    }
  }

  public void UpdateCell(int i, int j, int sum){
    if(sum == 3){
      next[i, j] = 1;
    }
    else if(sum < 2 || sum > 3){
      next[i, j] = 0;
    }
    else{
      next[i, j] = matrix[i, j];
    }
  }

  public void UpdateMatrix(){
    for(int i = 0; i < matrix.GetLength(0); i++){
      for(int j = 0; j < matrix.GetLength(1); j++){
        int sum = 0;
        for(int k = i - 1; k <= i + 1; k++){
          for(int l = j - 1; l <= j + 1; l++){
            if(k >= 0 && k < matrix.GetLength(0) && l >= 0 && l < matrix.GetLength(1) && (k != i || l != j)){
              if(matrix[k, l] == 1){
                sum++;
              }
            }
          }
        }
        UpdateCell(i, j, sum);
      }
    }
  }

  public void OnTimerTimeout(){
    UpdateMatrix();
    matrix = (int[,])next.Clone();
    timer.Start();
  }

  public void OnStartButtonPressed(){
    timer.Start();
    startButton.Disabled = true;
    stopButton.Disabled = false;
  }

  public void OnStopButtonPressed(){
    timer.Stop();
    stopButton.Disabled = true;
    startButton.Disabled = false;
  }

  public void OnSpeedSliderValueChanged(double value){
    timer.WaitTime = 1/value;
  }

  public override void _Input(InputEvent @event){
    if(@event is InputEventMouseButton mouseEvent){
      if(mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Middle){
        move = GetGlobalMousePosition();
        oldCamPos = cam.Position;
      }
      if(mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.WheelUp){
        cam.Zoom /= 0.8f;
      }
      if(mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.WheelDown){
        cam.Zoom *= 0.8f;
      }
    }
  }

    public override void _Process(double delta){
      DrawMatrix();
      if(Input.IsMouseButtonPressed(MouseButton.Left)){
        Vector2 globalMousPos = GetGlobalMousePosition()/32;
        this.matrix[(int)(globalMousPos.X), (int)(globalMousPos.Y)] = 1;
      }
      if(Input.IsMouseButtonPressed(MouseButton.Right)){
        Vector2 globalMousPos = GetGlobalMousePosition()/32;
        this.matrix[(int)(globalMousPos.X), (int)(globalMousPos.Y)] = 0;
      }
      if(Input.IsMouseButtonPressed(MouseButton.Middle)){
        cam.Position = oldCamPos - GetGlobalMousePosition() + move;
      }
    }
}
