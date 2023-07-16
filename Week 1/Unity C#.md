# Unity C#

## unity中的事件函数：

Awake：脚本实例被创建时调用（用于游戏对象的初始化，注意Awake的执行早于所有脚本的Start函数）

Start： Update函数第一次运行之前调用（用于游戏对象的初始化）

Reset： 用户点击检视面板的Reset按钮或者首次添加该组件时被调用。此函数只在编辑模式下被调用。Reset最常用于在检视面板中给定一个最常用的默认值。

Update：每帧调用一次（用于更新游戏场景和状态）

FixedUpdate：每个固定物理时间间隔调用一次（用于物理引擎的参数更新）

LateUpdate：每帧调用一次，在Update之后（用于更新游戏场景和状态，和相机有关的更新一般放在这里）

### Update、Fixed Update和LateUpdate

Update是每帧更新，FixedUpdate是固定间隔更新，LateUpdate在Update执行完之后执行。在Edit -> Project Setting -> Time中Fixed Timestep就是FixedUpdate的调用间隔

#### 为什么要用FixedUpdate：

比如，角色移动在Update中用速度乘于时间差Time.deltatime，当某帧卡顿了很久，Time.deltatime就会很大，导致角色瞬移。因此当需要有较短且稳定的更新时，可以写在FixedUpdate中。

##### 每帧中函数的执行顺序是FixedUpdate->Update->LateUpdate

##### 同样的运算量，FixedUpdate比Update更容易引起卡顿，不要在FixedUpdate中放太多的负荷（庞大的FixedUpdate会拉大帧间隔，导致每帧加入更多的FixedUpdate）

总的来说FixedUpdate管理物理系统、Update管理帧渲染前的游戏对象属性变化、LateUpdate函数大多用在摄像机的位置变化上

## 位移

![image text]https://github.com/Telluluu/QGStudioSummerLearning/blob/main/Week%201/Markdown%20Image/C%23/C%23%20image1.png



![](Markdown Image\C#\C# image1.png)

其中对刚体的操作，要放入FixedUpdate

另，Character Controller自带Capsule Collider

![image text](https://github.com/Telluluu/QGStudioSummerLearning/blob/main/Week%201/Markdown%20Image/C%23/C%23%20gif.gif)

用CharacterController组件做了一个简单的移动

## 动画：

创建Animator Controller![](Markdown Image\C#\C# image2.png)

在Animator面板中设定各种状态、转换条件

![](Markdown Image\C#\C# image3.png)

在脚本文件中更改Animator的Parameters来实现控制

![](Markdown Image\C#\C# image4.png)

## 预制件：

![](Markdown Image\C#\C# image5.png)

可以通过脚本实例化预制件的方式来动态实例化预制件，实现比如发射子弹的效果

核心函数，是`GameObject.Instantiate` 和 `GameObject.Destroy`

```c#
public class CreateBullet : MonoBehaviour
{

    public GameObject bulletPrefab;  //子弹预制体
    void Update()
    {
        Shoot();
    }
    void Shoot()
    {
        if (Input.GetMouseButtonDown(0))  //如果按下鼠标左键，生成预制体
        {
            Instantiate(bulletPrefab, transform.position, transform.rotation);  //生成预制体
        }
    }
}
```

这里让子弹在发射器的位置生成（不过没有添加速度）

要销毁物体可以使用Destroy(gameObject)或者DestroyImmediate(gameObject)

前者在下一帧销毁，后者立即销毁，官方推荐使用Destroy代替DestroyImmediate

原因是DestroyImmediate是立即销毁，立即释放资源，做这个操作的时候，会消耗很多时间的，影响主线程运行
Destroy是异步销毁，一般在下一帧就销毁了，不会影响主线程的运行。

## 一些重要的类：

### Transfrom：

#### 变量：

position：The position of the transfrom in world space.在世界空间坐标transform的位置。
 localposition: The position of the transfrom relative to parent transfrom.相对于父级的变换的位置。
 LossyScale：相对于模型的，物体的全局缩放（只读）。
 LocalScale：相对于父级物体变换的缩放。

#### 常用函数：

Translate()

Rotate()

RotateAround()

### Object:

#### 常用函数：

Destroy(gameObject) 销毁物体

Destroy(gameObject, 5) 5秒后销毁物体

Instantiate(projectile, transform.position, transform.rotation) 实例化

### GameObject：

#### 变量：

active 是否活动 

isStatic 是否静态

#### 常用函数：

AddComponent("name")

GetComponent(name)

Find("name") 找到并返回名为name的物体

FindWithTag("name") 找到并返回带有名为name的tag的物体

SetActive(true) / SetActive(false) 显示/隐藏游戏对象

### Component：

#### 常用函数：

GetComponment()
 GetComponmentInChildren()
 GetComponmentInParent()
 GetComponmenstInChildren()
 GetComponmentsInParent()

### Time:

用于测量和控制时间，并管理项目的帧率

Time.deltaTime 帧间隔时间

### Mathf: 

一些常见的数学函数