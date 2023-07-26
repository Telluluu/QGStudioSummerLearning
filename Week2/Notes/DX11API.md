# DX11API

## 输入布局(Input Layout)：

`ID3D11Device::CreateInputLayout`创建输入布局

在HLSL中用于输入的结构体为：

```HLSL
struct VertexIn
{
    float3 pos : POSITION;
    float4 color : COLOR;
};
```

与C++中的结构体对应

```c++
struct VertexPosColor
{
    DirectX::XMFLOAT3 pos;
    DirectX::XMFLOAT4 color;
    static const D3D11_INPUT_ELEMENT_DESC inputLayout[2];
};
```

要建立起二者的联系，需要使用ID3D11InputLayout输入布局来描述每一个成员的用途、语义、大小等信息，我们使用D3D11_INPUT_ELEMENT_DESC来描述传入结构体中每个成员的信息

```c++
typedef struct D3D11_INPUT_ELEMENT_DESC
{
    LPCSTR SemanticName;        // 语义名
    UINT SemanticIndex;         // 语义索引
    DXGI_FORMAT Format;         // 数据格式
    UINT InputSlot;             // 输入槽索引(0-15)
    UINT AlignedByteOffset;     // 初始位置(字节偏移量)
    D3D11_INPUT_CLASSIFICATION InputSlotClass; // 输入槽类别(顶点/实例)
    UINT InstanceDataStepRate;  // 实例数据步进值
}     D3D11_INPUT_ELEMENT_DESC;
```

```c++
const D3D11_INPUT_ELEMENT_DESC VertexPosColor::inputLayout[2] = {
    { "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
    { "COLOR", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0}
};
```

<font size=5>语义名</font>与HLSL中的语义名相同

当存在多个相同的语义时，从上到下分别以0，1，2...作为<font size=5>语义索引</font>以区分。

<font size=5>数据格式</font>描述数据的存储方式、大小，如上述例子中POSITION以3个float类型的值存储，COLOR以RBGA存储

<font size=5>初始位置</font>是指该成员位置与初始成员所在的字节偏移量

<font size=5>输入类型</font>包括`D3D11_INPUT_PER_VERTEX_DATA`为按每个顶点数据输入，`D3D11_INPUT_PER_INSTANCE_DATA`则是按每个实例数据输入。

接下来就可以使用`ID3D11Device::CreateInputLayout`来创建一个输入布局

```c++
HRESULT ID3D11Device::CreateInputLayout( 
    const D3D11_INPUT_ELEMENT_DESC *pInputElementDescs, // [In]输入布局描述
    UINT NumElements,                                   // [In]上述数组元素个数
    const void *pShaderBytecodeWithInputSignature,      // [In]顶点着色器字节码
    SIZE_T BytecodeLength,                              // [In]顶点着色器字节码长度
    ID3D11InputLayout **ppInputLayout);                 // [Out]获取的输入布局
```

##### 示例↓

```c++
const D3D11_INPUT_ELEMENT_DESC GameApp::VertexPosColor::inputLayout[2] = {
    { "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
    { "COLOR", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 }
};

bool GameApp::InitEffect()
{
    ComPtr<ID3DBlob> blob;

    // 创建顶点着色器//
    
    // 创建并绑定顶点布局
    HR(m_pd3dDevice->CreateInputLayout(VertexPosColor::inputLayout, ARRAYSIZE(VertexPosColor::inputLayout),
        blob->GetBufferPointer(), blob->GetBufferSize(), m_pVertexLayout.GetAddressOf()));

    // 创建像素着色器//
    return true;
}

bool GameApp::InitResource()
{
    //others//
        m_pd3dImmediateContext->IASetInputLayout(m_pVertexLayout.Get());
    //others//
}
```

## 顶点/像素着色器的创建：

在D3D设备可以创建出6中着色器：

| 方法                               | 着色器类型           | 描述       |
| ---------------------------------- | -------------------- | ---------- |
| ID3D11Device::CreateVertexShader   | ID3D11VertexShader   | 顶点着色器 |
| ID3D11Device::CreateHullShader     | ID3D11HullShader     | 外壳着色器 |
| ID3D11Device::CreateDomainShader   | ID3D11DomainShader   | 域着色器   |
| ID3D11Device::CreateComputeShader  | ID3D11ComputeShader  | 计算着色器 |
| ID3D11Device::CreateGeometryShader | ID3D11GeometryShader | 几何着色器 |
| ID3D11Device::CreatePixelShader    | ID3D11PixelShader    | 像素着色器 |

这些方法的输入形参都是一致的，只是输出的是不同着色器，以创建顶点着色器为例：

```c++
HRESULT ID3D11Device::CreateVertexShader( 
    const void *pShaderBytecode,            // [In]着色器字节码
    SIZE_T BytecodeLength,                  // [In]字节码长度
    ID3D11ClassLinkage *pClassLinkage,      // [In_Opt]忽略
    ID3D11VertexShader **ppVertexShader);   // [Out]获取顶点着色器
```

```c++
    ComPtr<ID3DBlob> blob;

    // 创建顶点着色器
    HR(CreateShaderFromFile(L"HLSL\\Triangle_VS.cso", L"HLSL\\Triangle_VS.hlsl", "VS", "vs_5_0", blob.ReleaseAndGetAddressOf()));
    HR(m_pd3dDevice->CreateVertexShader(blob->GetBufferPointer(), blob->GetBufferSize(), nullptr, m_pVertexShader.GetAddressOf()));
```

这里先使用`CreateShaderFromFile`函数获取着色器二进制信息，然后再调用`CreateVertexShader`创建着色器

## 顶点缓冲区的创建：

缓存区的描述：

```c++
typedef struct D3D11_BUFFER_DESC
{
    UINT ByteWidth;             // 数据字节数
    D3D11_USAGE Usage;          // CPU和GPU的读写权限相关
    UINT BindFlags;             // 缓冲区类型的标志
    UINT CPUAccessFlags;        // CPU读写权限的指定
    UINT MiscFlags;             // 忽略
    UINT StructureByteStride;   // 忽略
}     D3D11_BUFFER_DESC;
```

|                       | CPU读 | CPU写 | GPU读 | GPU写 |
| --------------------- | :---: | :---: | :---: | :---: |
| D3D11_USAGE_DEFAULT   |       |       |   √   |   √   |
| D3D11_USAGE_IMMUTABLE |       |       |   √   |       |
| D3D11_USAGE_DYNAMIC   |       |   √   |   √   |       |
| D3D11_USAGE_STAGING   |   √   |   √   |   √   |   √   |

<font size=5>`D3D11_USAGE_DEFAULT`</font>:只能由GPU读写，更新块

<font size=5>`D3D11_USAGE_DYNAMIC`</font>:可以直接获取来自显存的数据，但代价就是更新效率更低，应当使用`ID3D11DeviceContext::Map`和`ID3D11DeviceContext::Unmap`来将显存中的数据映射到内存中，然后修改该片内存中的数据，最后将修改好的数据映射回显存中

例：

```c++
// 设置三角形顶点
// 注意三个顶点的给出顺序应当按顺时针排布
VertexPosColor vertices[] =
{
    { XMFLOAT3(0.0f, 0.5f, 0.5f), XMFLOAT4(0.0f, 1.0f, 0.0f, 1.0f) },
    { XMFLOAT3(0.5f, -0.5f, 0.5f), XMFLOAT4(0.0f, 0.0f, 1.0f, 1.0f) },
    { XMFLOAT3(-0.5f, -0.5f, 0.5f), XMFLOAT4(1.0f, 0.0f, 0.0f, 1.0f) }
};
// 设置顶点缓冲区描述
D3D11_BUFFER_DESC vbd;
ZeroMemory(&vbd, sizeof(vbd));
vbd.Usage = D3D11_USAGE_IMMUTABLE;
vbd.ByteWidth = sizeof vertices;
vbd.BindFlags = D3D11_BIND_VERTEX_BUFFER;
vbd.CPUAccessFlags = 0;
```

然后需要使用<font size=5>`D3D11_SUBRESOURCE_DATA`</font>来指定要用来初始化的数据

```c++
typedef struct D3D11_SUBRESOURCE_DATA
{
    const void *pSysMem;        // 用于初始化的数据
    UINT SysMemPitch;           // 忽略
    UINT SysMemSlicePitch;      // 忽略
}     D3D11_SUBRESOURCE_DATA;
```

填充子资源数据结构体：

```c++
// 新建顶点缓冲区
D3D11_SUBRESOURCE_DATA InitData;
ZeroMemory(&InitData, sizeof(InitData));
InitData.pSysMem = vertices;
```

最后通过<font size=5>`ID3D11Device::CreateBuffer`</font>来创建一个缓冲区

```c++
HRESULT ID3D11Device::CreateBuffer( 
    const D3D11_BUFFER_DESC *pDesc,     // [In]顶点缓冲区描述
    const D3D11_SUBRESOURCE_DATA *pInitialData, // [In]子资源数据
    ID3D11Buffer **ppBuffer);           // [Out] 获取缓冲区

```

```c++
ComPtr<ID3D11Buffer> m_pVertexBuffer = nullptr;
HR(m_pd3dDevice->CreateBuffer(&vbd, &InitData, m_pVertexBuffer.GetAddressOf()));
```

之后就可以再输入装配阶段设置该顶点缓冲区了：

```c++
void ID3D11DeviceContext::IASetVertexBuffers( 
    UINT StartSlot,     // [In]输入槽索引
    UINT NumBuffers,    // [In]缓冲区数目
    ID3D11Buffer *const *ppVertexBuffers,   // [In]指向缓冲区数组的指针
    const UINT *pStrides,   // [In]一个数组，规定了对所有缓冲区每次读取的字节数分别是多少
    const UINT *pOffsets);  // [In]一个数组，规定了对所有缓冲区的初始字节偏移量

```

```c++
// 输入装配阶段的顶点缓冲区设置
UINT stride = sizeof(VertexPosColor);    // 跨越字节数
UINT offset = 0;                        // 起始偏移量
    
m_pd3dImmediateContext->IASetVertexBuffers(0, 1, m_pVertexBuffer.GetAddressOf(), &stride, &offset);
```

## 常量缓冲区(Constant Buffer/C-Buffer)

在HLSL中，常量缓冲区类似于C++中的全局常量，供着色器代码使用

***<font size=6>在创建常量缓冲区时，描述参数`ByteWidth`必须为16的倍数，因为HLSL的常量缓冲区本身以及对它的读写操作需要严格按16字节对齐</font>***

```HLSL
cbuffer ConstantBuffer:register(b0)
{
	matrix g_World;
	matrix g_View;
	matrix g_Proj;
}
```

在C++中，常量缓冲区对应的结构体为

```c++
struct ConstantBuffer
{
	XMMATRIX world;
	XMMATRIX view;
	XMMATRIX proj;
};
```

常量缓冲区由两种运行时更新方式：

1. 指定`Usage`为`D3D11_USAGE_DEAFAULT`,需要用`ID3D11DeviceContext::UpdateSubresource`更新
2. 指定`Usage`为`D3D11_USAGE_DYNAMIC`,`CPUAccessFlags`为`D3D11_CPU_ACCESS_WRITE`允许常量缓冲区从CPU写入，首先通过`ID3D11DeviceContext::Map`获取内存映射，然后再更新到映射好的内存地址，最后通过`ID3D11DeviceContext::Unmap`解除映射

一般的缓冲区和纹理资源更新也可以使用上述两种方式，谦和适合更新不频繁，或仅更新一次的数据，后者适合需要频繁更新的数据，如每几帧更新一次，或每帧更新一次或多次的资源