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
  HSlider speedSlider;

  public override void _Ready(){
    grid = (TileMapLayer)GetNode("TileMapLayer");
    cam = (Camera2D)GetNode("Camera2D");
    timer = (Timer)GetNode("Timer");
    timer.Timeout += OnTimerTimeout;
    startButton = (Button)GetNode("CanvasLayer/StartButton");
    startButton.Pressed += OnStartButtonPressed;
    speedSlider = (HSlider)GetNode("CanvasLayer/SpeedSlider");
    speedSlider.ValueChanged += OnSpeedSliderValueChanged;
    matrix = new int[100, 100];
    matrix[5, 5] = 1;
    matrix[5, 6] = 1;
    matrix[5, 7] = 1;
    next = new int[100, 100];
  }
  
  public void SetCell(int i, int j, int TileId){
    this.grid.SetCell(new Vector2I(i, j), 0, new Vector2I(TileId, 0));
  }

  public void DrawMatrix(){
    Vector2 viewportSize = cam.GetViewportRect().Size/32;
    Vector2 cameraPos = cam.GlobalPosition/32;
    for(int i = (int)(cameraPos.X - viewportSize.X/2); i < Math.Ceiling(cameraPos.X + viewportSize.X/2); i++){
      for(int j = (int)(cameraPos.Y - viewportSize.Y/2); j < Math.Ceiling(cameraPos.Y + viewportSize.Y/2); j++){
        SetCell(i, j, this.matrix[i, j]);
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
    // foreach(int i in new List<int> {0, matrix.GetLength(0) - 1}){
    //   foreach(int j in new List<int> {0, matrix.GetLength(1) - 1}){
    //
    //   }
    // }
    // int sum = matrix[0, 1] + matrix[1, 0] + matrix[1, 1];
    // UpdateCell(0, 0, sum);
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
  }

  public void OnSpeedSliderValueChanged(double value){
    timer.WaitTime = 1/value;
  }

  public override void _Input(InputEvent @event){
    if(@event is InputEventMouseButton mouseEvent){
      if(mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left){
        Vector2 globalMousPos = GetGlobalMousePosition()/32;
        this.matrix[(int)(globalMousPos.X), (int)(globalMousPos.Y)] = 1;
      }
      if(mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Right){
        Vector2 globalMousPos = GetGlobalMousePosition()/32;
        this.matrix[(int)(globalMousPos.X), (int)(globalMousPos.Y)] = 0;
      }
    }
  }

    public override void _Process(double delta){
      DrawMatrix();
    }
}
