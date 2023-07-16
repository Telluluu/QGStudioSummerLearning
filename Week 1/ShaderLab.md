# ShaderLab

## 1.ShaderLab的结构

Shader "name" { 

[Properties] 

SubShaders 

[FallBack] 

[CustomEditor] 

}

[]表示该部分是可选的

1. Shader "name"  //Shader的路径名称
2. Properties  //属性，材质球面板中显示的贴图和一些参数什么的都是在此Properties中进行定义设置的，内容必须写在Properties后的{}内。
3. SubShaders  //子着色器，在加载Shader时，Unity会遍历所有SubShader，并选择支持用户机器的第一个
4. FallBack  //如果SubShader中所有Shader都不支持，则会选用FallBack路径的Shader
5. CustomEditor  //自定义界面，也就是说我们可以通过这个功能来自由定义材质面板的显示结果，它可以改写Properties中定义的显示方式。

## 2.SubShaders



```ShaderLab
SubShader{
    Pass{
    	CGPROGRAM
   	    #pragma vertex vert
		#pragma fragment frag

		float4 vert(float4 v:POSITION):SV_POSITION{
			return UnityObjectToClipPos(v);
		}

		fixed4 frag():SV_Target{
			return fixed4(1.0,1.0,1.0,1.0);
		}
		ENDCG
	}
}
```

示例↑

### 编译指令

   	    #pragma vertex name
   		#pragma fragment name

这两行编译指令告诉Unity哪个函数包含了顶点着色器的代码，哪个函数包含了片元着色器的代码。name是我们指定的函数名，可以是任意的合法函数名，一般使用vert和frag

### 顶点着色器

```ShaderLab
	float4 vert(float4 v:POSITION):SV_POSITION{
		return UnityObjectToClipPos(v);
	}
```

输入参数v包含了顶点的位置（逐顶点输入），POSITON和SV_POSITION是CG/HLSL中的语义，是不可省略的，其作用是告诉系统用户需要输入哪些值。如上述例子中，POSTION告诉Unity，把模型的顶点坐标填充到输入参数v中，SV_POSITION告诉Unity，顶点着色器的输出是裁剪空间中的顶点坐标

### 片元着色器

		fixed4 frag():SV_Target{
			return fixed4(1.0,1.0,1.0,1.0);
		}

这里的frag函数没有任何输入，它输入一个fixed4类型的变量（低精度RGBA颜色），且使用了SV_Target语义进行限定，这里将输出到默认的帧缓存中。片元着色器中的代码表示返回一个白色的fixed4



如果我们想要得到更多的模型数据，可以为顶点着色器定义一个结构体输入参数

```ShaderLab
			struct v2f
			{
				float4 pos:SV_POSITION;
				float3 color:COLOR0;
			};
```

我们还定义了一个结构体用于顶点着色器和片元着色器之间的通信

```ShaderLab
			struct v2f
			{
				float4 pos:SV_POSITION;
				float3 color:COLOR0;
			};
```

然后在vert中使用结构体作为参数输入

```ShaderLab
			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				//v.normal包含了顶点的法线方向，其分量范围在[-1.0,1.0]
				//下面的代码把分量范围映射到了[0.0,1.0]
				//存储到o.color中传递给片元着色器
				o.color = v.normal * 0.5 + float3(0.5,0.5,0.5);
				return o;
			}
```

```ShaderLab
			fixed4 frag(v2f i):SV_Target
			{
				return fixed4(i.color,1.0);
			}
```

#### 注意的点：

顶点着色器是逐顶点调用的，而片元着色器是逐片元调用的，片元着色器中的输入实际上是把顶点着色器的输入进行插值后得到的结果

## 3.Properties：

在unity中，shader绑定在material上，通过material上的参数，我们可以随时调整material的效果，而这些参数就需要写在Shader的Properties语义块中

```ShaderLab
	Properties{
		//声明一个Color类型的属性
		_Color("Corlor Tint",Color)=(1.0,1.0,1.0,1.0)
	}
```

引号中的是Material的Inspector面板中对应属性显示的名称

在Pass中也要定义一个与属性名称和类型都匹配的变量

​    `fixed4 _Color;`

```ShaderLab
			fixed4 frag(v2f i):SV_Target
			{
				fixed3 c = i.color;
				//使用_Color属性来控制输出颜色
				c=c*_Color.rgb;
				return fixed4(c,1.0);
			}
```

在frag函数中用_Color属性来控制输出颜色

![image-20230712111917101](D:\OneDrive\学校\大学\QG工作室图形组\2023训练营\2023暑期训练营\Week 1\ShaderLab image1.png)

![image-20230712155352643](D:\OneDrive\学校\大学\QG工作室图形组\2023训练营\2023暑期训练营\Week 1\ShaderLab image2.png)

## 附.

### Unity提供的内置文件和变量

![image-20230712155552369](D:\OneDrive\学校\大学\QG工作室图形组\2023训练营\2023暑期训练营\Week 1\ShaderLab image3.png)

![image-20230712155618979](D:\OneDrive\学校\大学\QG工作室图形组\2023训练营\2023暑期训练营\Week 1\ShaderLab image4.png)

![image-20230712155653486](D:\OneDrive\学校\大学\QG工作室图形组\2023训练营\2023暑期训练营\Week 1\ShaderLab image5.png)

### Unity 提供的CG/HLSL语义

![image-20230712160029629](D:\OneDrive\学校\大学\QG工作室图形组\2023训练营\2023暑期训练营\Week 1\Markdown Image\ShaderLab\ShaderLab image6.png)

![image-20230712160108674](D:\OneDrive\学校\大学\QG工作室图形组\2023训练营\2023暑期训练营\Week 1\Markdown Image\ShaderLab\ShaderLab image7.png)

