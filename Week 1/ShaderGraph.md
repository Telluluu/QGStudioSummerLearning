# ShaderGraph

ShaderGraph是unity中的可视化着色器构建工具，目前只兼容URP和HDRP

## 以URP为例：

在Asset中新建一个ShaderGraph

![image text](https://github.com/Telluluu/QGStudioSummerLearning/blob/main/Week%201/Markdown%20Image/ShaderGraph/ShaderGraph%20image1.png)

在右侧的Graph Inspector中可以对Shader做一些设置

### 操作：

最左侧的菜单可以新建输入

![image text](https://github.com/Telluluu/QGStudioSummerLearning/blob/main/Week%201/Markdown%20Image/ShaderGraph/ShaderGraph%20image2.png)

这里新建了一个Color用于控制颜色

![image text](https://github.com/Telluluu/QGStudioSummerLearning/blob/main/Week%201/Markdown%20Image/ShaderGraph/ShaderGraph%20image3.png)

还可以输入一个纹理，通过纹理采样器来改变颜色

![image text](https://github.com/Telluluu/QGStudioSummerLearning/blob/main/Week%201/Markdown%20Image/ShaderGraph/ShaderGraph%20image4.png)

![image text](https://github.com/Telluluu/QGStudioSummerLearning/blob/main/Week%201/Markdown%20Image/ShaderGraph/ShaderGraph%20image5.png)

通过multiply节点叠加效果

![image-20230714133113278](Markdown Image\ShaderGraph\ShadeGraph image6.png)

添加了一个白平衡调整，同理还能添加别的效果，比如饱和度

各种Node节点组合起来，完成我们需要的效果

### SubGraph:

选中所有节点，右键convert to subgraph可以将将这些节点保存，只保留输入输出，方便复用

![image-20230714134102664](Markdown Image\ShaderGraph\ShaderGraph image7.png)

## 一些效果：

### 边缘光：

只对头发部分进行了边缘光处理

![image-20230714154447368](Markdown Image\ShaderGraph\ShaderGraph image8.png)

![image-20230714154529499](Markdown Image\ShaderGraph\ShadeGraph image9.png)

使用菲涅尔效应，连接至Emission即可

### 溶解特效：

这里只对身体部分应用了溶解特效

![image-20230714170601419](Markdown Image\ShaderGraph\ShaderGraph image9.png)

主要思路：

利用噪声图生成Alpha，然后在溶解进度Clip上add一个边缘宽度EdgeWidth，用Step比对Alpha，如果Clip加上边缘宽度EdgeWidth比Alpha大，则边缘发光，发光颜色用一个Color属性来控制。上一个边缘光特效已经占用了Emission属性，所有我们把溶解边缘光和上一节的普通边缘光相加，再输出给Emission