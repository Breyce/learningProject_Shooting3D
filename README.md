# Shooting3D

## Development Day 1: 2023.11.28

1. `[RequireComponent (typeof(PlayerController))]`：当某个脚本必须依赖其他脚本或者组件共同使用时，为了避免人为添加过程的操作失误，可以在代码中使用`RequireComponent`，它的作用就是**添加该脚本**时，会**自动将所依赖的各个组件添加至`gameobject`上**，避免人为操作的失误。

2. 俯视视角让**玩家的视角追随鼠标光标**，这里使用的办法是：
   1. 从Camera发射一道经过鼠标的射线；

      ```
      Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
      ```

   2. 与地面进行检测；

      ```
      Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
      float rayDistance;
      if(groundPlane.Raycast(ray, out rayDistance)){...}
      ```

   3. 如果射线检测检测到位置，就让玩家面向那个方向。

      ```
      Vector3 point = ray.GetPoint(rayDistance);
      
      controller.LookAt(point);
      ```

      

3. 想在界面当中调试时可视化射线，可以用`Debug.DrawLine(ray.origin, point, Color.red);`

4. `    public event System.Action OnDeath`：

   涉及到C#的【[事件（1）](https://blog.csdn.net/kokool/article/details/129772271)】【[事件（2）](https://blog.csdn.net/Mr_Sun88/article/details/83689638)】【[委托](https://blog.csdn.net/weixin_45775438/article/details/128449023?spm=1001.2101.3001.6650.6&utm_medium=distribute.pc_relevant.none-task-blog-2%7Edefault%7EBlogCommendFromBaidu%7ERate-6-128449023-blog-83689638.235%5Ev38%5Epc_relevant_anti_t3_base&depth_1-utm_source=distribute.pc_relevant.none-task-blog-2%7Edefault%7EBlogCommendFromBaidu%7ERate-6-128449023-blog-83689638.235%5Ev38%5Epc_relevant_anti_t3_base&utm_relevant_index=7)】，具体可以通过这几篇CSDN链接来看，讲的很清楚。

   简单来说，在一个里面定义好发布者，然后其他的人是订阅者。在这个项目当中:

   - `LivingEntity`是事件的发布者：

   ```
   public event System.Action OnDeath;
   
   ---------------小tip---------------
   public delegate void MyDelegate();
   public event MyDelegate myEvent;
   
   上述两个式子可以简写为：public event Action myEvent;
   
   即：public event System.Action OnDeath 可写为
   public delegate void System.Action();
   public event System.Action OnDeath;
   -----------------------------------
   
   OnDeath();
   ```

   - `Spawner`和`Enemy`里面的`spawnEnemy`和`targetEntity`是订阅者，订阅者是需要继承`LivingEntity`的：

   ```
   #Spawner.cs
   Enemy spawnEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
   spawnEnemy.OnDeath += OnEnemyDeath;
   
   #Enemy.cs
   targetEntity = target.GetComponent<LivingEntity>();
   targetEntity.OnDeath += OnTargetDeath;
   ```

   - 具体执行就是，`Spawner`和`Enemy`在定义的`OnDeath`活动发生的时候，要分别执行不同的操作，因此，让两个订阅者的信息挂到发布者身上，就可以每当`OnDeath`出发的时候，同时执行订阅者挂载上来的函数。

5. 当敌人很多时，运算距离使用Vector3.Distance是相当消耗算力的。所以可以采用另一个方法，将欧式几何距离改为向量模：

   ```
   float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
   ```

6. `Mathf.Pow(attackDistanceThreshold, n)`：求`attackDistanceThreshold`的`n`次方。

7. 令敌人的追踪目的地在角色的半径之外：

   ```
   float myCollisionRadius;
   float targetCollisionRadius;
       
   //在代码中...
   {
      	Vector3 dirToTaeget = (target.position - transform.position).normalized;
   	Vector3 targetPosition = target.position - dirToTaeget * (myCollisionRadius + targetCollisionRadius);
       
       pathFinder.SetDestination(targetPosition); // 赋值
   }  
   ```

8. 解决的问题：子弹因为穿模在游戏实体内部打出，此时射线无法检测到，那么会浪费一颗子弹。
   解决方法：在子弹生成时，检测半径为0.1f的一个球体范围内是否与Enemy层有重叠，有，则说明子弹打到敌人了。

## Development Day 2: 2023.11.29

1. 编写一个`Unity3D`的`Editor`，需要调用`using UnityEditor`，并且声明当前哪个脚本是`Editor`，在编辑器当中，当前这个脚本用`target`指代：

   ```
   using System.Collections;
   using System.Collections.Generic;
   using UnityEditor;
   using UnityEngine;
   
   [CustomEditor(typeof(MyEditor))]
   public class Editor : Editor
   {
   	/* 编辑器代码 */
   }
   ```

2. 制作一个工具类：

   ```
   using System.Collections;
   
   public static class Utility
   {
   	/* 工具类代码 */
   }
   ```

3. `obstacleMap.GetLength(0)`



## Development Day 3: 2023.12.2

1. 内部类如果想要在UI上显示出来，需要给他添加`[System.Serializable]`，并且保证类中所有类型都是可显示的。如下所示，内部类中所有的`Coordinate`类型的参数都需要在`Coordinate`上添加`[System.Serializable]`才行，实现可序列化：

   ```C#
       [System.Serializable]
       public struct Coordinate
       {
       	//...
       }
   
       [System.Serializable]
       public class Map
       {
           public Coordinate mapSize;
           [Range(0,1)]
           public float obstaclePercent;
           public int seed;
           
           public Coordinate mapCenter
           {
               get
               {
                   return new Coordinate(mapSize.x/2, mapSize.y/2);
               }
           }
       }
   ```

2. 使用`DrawDefaultInspector();`可以保证只有在编辑器当中的值更新时才会调用，从而避免了不断地调用这个值影响`Unity`的效率。

3. 通过`[ContextMenu("Your Tip")]`可以再`Unity`的UI当中右键时添加一个选项，这个选项会触发其下绑定的函数。