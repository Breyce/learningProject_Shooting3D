# Shooting3D

## Development Day 1: 2023.11.28

1. `[RequireComponent (typeof(PlayerController))]`：当某个脚本必须依赖其他脚本或者组件共同使用时，为了避免人为添加过程的操作失误，可以在代码中使用`RequireComponent`，它的作用就是**添加该脚本**时，会**自动将所依赖的各个组件添加至`gameobject`上**，避免人为操作的失误。
2. 俯视视角让**玩家的视角追随鼠标光标**，这里使用的办法是：
   1. 从Camera发射一道经过鼠标的射线；
   2. 与地面进行检测；
   3. 如果射线检测检测到位置，就让玩家面向那个方向。
3. 想在界面当中调试时可视化射线，可以用`Debug.DrawLine(ray.origin, point, Color.red);`