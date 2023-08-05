# FFT海面模拟

![](D:\OneDrive\学校\大学\QG工作室图形组\2023训练营\2023暑期训练营\Week 2~3\Markdown Images\FFT海面模拟image1.jpg)

在数学中，傅里叶变换将一个函数分解成它的组成频率。

在上图中，从左侧观看得到的是时域图像，而从右侧观看得到的时频率图像，傅里叶变换就能将时域图像变换为频域图像，相反的，傅里叶逆变换可以将频域图像转换为时域图像。

在海面模拟中，我们需要生成一个频域图像，通过傅里叶逆变换将其转换为时域图像

## 离散傅里叶变换与离散傅里叶逆变换

计算机不能处理连续的数据，因此我们需要离散傅里叶变换
$$
F(μ)=\sum_{x=0}^{N-1}​f(x)e^{−iN2πμx\over N}​
$$
相应的，离散傅里叶逆变换的公式为
$$
f(x)= 
{1\over N}​\sum_{μ=0}^{N−1}​F(μ)e^{i2πμx\over N}​
$$

## 生成频谱

我们需要生成一个频谱，再进行离散傅里叶逆变换

在**海洋统计学模型**中，波高被认为是**水平位置和时间**的随机变量h（x，t）

x=（x,z)为空域坐标，k=(kx,kz)为频域坐标
$$
h(\vec x,t)=\sum_\vec k​\widetilde h(\vec k,t)e^{i\vec  k\vec x}
$$

$$
其中\widetilde h(k,t)e^{i\widetilde k\vec x}通常采用菲利普频谱
$$

$$
\widetilde h(k,t)e^{i\widetilde k\vec x}=\widetilde h_0(\vec k)e^{i\omega(k)t}+\widetilde h_0^*(-\vec k)e^{-i\omega(k)t} \\
其中\widetilde h_0^*为\widetilde h_0的共轭复数，k表示\widetilde k的模
$$

$$
\\
\omega(k) = \sqrt{gk} \\ g为重力加速度
$$




$$
\widetilde h_0(\vec k) ={ {1\over \sqrt{2}}(\zeta_r+\zeta_i)\sqrt{P_h(\vec k )}} \\ \zeta_r和\zeta_i是相互独立的随机数，均服从均值为0，标准差为1的正态分布
$$



$$
P_h(\vec k) = A{e^{-1\over{kL}^2}\over k^4} |{\vec k^2·\vec \omega}|^2 \\
L = {V^2\over g} ，这里的L不同于海面patch尺寸，V为风速，g为重力加速度
\\
|{\vec k^2·\vec \omega}|^2为方向拓展，取决于风的方向
\\
然而该公式在波数多时拟合度较差，在Tessendorf的论文中提到可以将|{\vec k^2·\vec \omega}|^2修改为exp(-k^2l^2),其中l<<L
\\
P_h(\vec k) = A{e^{-1\over{kL}^2}\over k^4}e^{-k^2l^2}, \text l<<L
$$

## FFT算法

对于标准DFT：
$$
X(K) = \sum_{n=0}^{N-1}x(n)e^{-i{2\pi k\over N}n},\text k\in{\{0,1,..,N-1\}} \\
为书写方便，令W_N^k=e^{-i{2\pi k\over N}}
$$
直接按DFT定义式计算，算法复杂度为O(N\*N)，快速傅里叶变换使用分治思想对DFT进行计算，可以将算法复杂度将至O(N\* log_2 N)

![](D:\OneDrive\学校\大学\QG工作室图形组\2023训练营\2023暑期训练营\Week 2~3\Markdown Images\FFT海面模拟image2.jpg)

将序号为偶数的输入给第一个N/2 point DFT calculator，序号为奇数的输入给第二个N/2 point DFT calculator

则有
$$
G(k)=\sum_{n=0}^{{N\over2}-1}g(n)e^{{-i{2\pi k}\over {N\over 2}}n}=\sum_{n=0}^{{N\over2}-1}x(2n)e^{{-i{2\pi k}\over {N\over 2}}n} ,\text k\in{\{0,1,..,N-1\}}
\\
H(k)=\sum_{n=0}^{{N\over2}-1}h(n)e^{{-i{2\pi k}\over {N\over 2}}n}=\sum_{n=0}^{{N\over2}-1}x(2n+1)e^{{-i{2\pi k}\over {N\over 2}}n} ,\text k\in{\{0,1,..,N-1\}}
$$
经过推导，可得
$$
X(k)=\begin {cases}
G(k)+W_N^kH(k) & k\in{\{0,1,...,{N\over 2} -1\}}
\\
G({k-N\over 2})+W_N^kH({k-N\over 2}) & k\in{\{{N\over 2},{N\over 2}+1,...,N -1\}}
\end{cases}
$$
![](D:\OneDrive\学校\大学\QG工作室图形组\2023训练营\2023暑期训练营\Week 2~3\Markdown Images\FFT海面模拟image3.png)

###### 补全算法图

但是递归一般效率不佳，不适合在GPU上实现，且在HLSL中不支持递归，于是我们需要采用展平的形式，即蝶形网络

## IFFT算法

我们将频域图像转换为时域图像使用的是逆傅里叶变换，不能直接套用FFT算法

但比较DFT和IDFT，二者很相似
$$
DFT:X(k)=\sum_{n=0}^{N-1}x(n)e^{-i{2\pi k n}\over N}, \text k\in {\{0,1,...,N-1\}} 
\\
IDFT:x(n)={1\over N}\sum_{k=0}^{N-1}X(k)e^{i{2\pi kn}\over N}, \text k\in {\{0,1,...,N-1\}} 
$$
![](D:\OneDrive\学校\大学\QG工作室图形组\2023训练营\2023暑期训练营\Week 2~3\Markdown Images\FFT海面模拟image4.png)

我们模仿FFT算法，用两个N/2 point IDFT calculator去构造一个N point IDFT calculator，将偶数序号的输入给第一个N/2 point IDFT calculator，奇数序号的输入给第二个N/2 point IDFT calculator

于是得到
$$
G(n)={1\over N}\sum_{k=0}^{{N\over 2}-1}g(k)e^{i{{2\pi kn}\over{N\over 2}}}={1\over N}\sum_{k=0}^{{N\over 2}-1}x(2k)e^{i{{2\pi kn}\over{N\over 2}}}, \text n\in{\{0,1,...,{N\over 2}-1\}}
\\
H(n)={1\over N}\sum_{k=0}^{{N\over 2}-1}h(k)e^{i{{2\pi kn}\over{N\over 2}}}={1\over N}\sum_{k=0}^{{N\over 2}-1}x(2k+1)e^{i{{2\pi kn}\over{N\over 2}}}, \text n\in{\{0,1,...,{N\over 2}-1\}}
$$
类似FFT的推到，可以得到
$$
x(n)=\begin{cases}
G(n)+W_N^{-n}H(n) & 
n\in{\{0,1,...,{N\over 2}-1\}}
\\
G({n-{N\over 2}})+W_N^{-n}({n-{N\over 2}}) &
n\in{\{{N\over 2},{N\over 2}+1,...N-1\}}
\end{cases}
$$
![](D:\OneDrive\学校\大学\QG工作室图形组\2023训练营\2023暑期训练营\Week 2~3\Markdown Images\FFT海面模拟image5.png)

###### 补全算法图

由于DFT/IDFT是线性的，所有常数因子不会影响算法

故适用于标准IDFT：
$$
x(n)={1\over N}\sum_{n=0}^{N-1}X(k)e^{i{{2\pi kn}\over N}}, \text n\in{\{0,1,...,N-1\}}
$$
的IFFT算法，可以不加修改地应用于为归一化的IDFT：
$$
x(n)=\sum_{n=0}^{N-1}X(k)e^{i{{2\pi kn}\over N}}, \text n\in{\{0,1,...,N-1\}}
$$
 海面的IDFT模型更接近于后者

## bitreverse算法

蝶形网络的N个输入的顺序是被打乱的，对于一般N point的情况，可以通过bitreverse算法计算出来

对于N point蝶形网络，求x(k)在几号位，只需将k化为log_2 N位二进制数，然后将bit反序，再转回十进制，所得结果即为x(k)所在位号。

以8 point蝶形网络为例：

x(0)在0号位，x(1)在4号位，x(2)在2号位，x(3)在6号位，x(4)在1号位，x(5)在5号位，x(6)在3号位，x(7)在7位号。以8 point蝶形网络为例，我们求x(3)在几号位，将3化为 log_2 8=3 位二进制数得011，bit反序得110，将110化回十进制得6，所以x(3)在6号位。

## 利用IFFT计算海面IDFT模型

$$
h(\vec x,t)=\sum_\vec k​\widetilde h(\vec k,t)e^{i\vec k\vec x}
$$

将海面IDFT模型写成标量形式
$$
h(x, z, t)=\sum_{m=-{N\over2}}^{{N\over 2}-1}\sum_{n=-{N\over2}}^{{N\over 2}-1}\widetilde h(k_x, k_z, t)e^{i(k_xx+k_zz)}
$$

$$
展开k_x和k_z
\\
k_x={2\pi n\over L}, \text n\in{\{-{N\over2},-{N\over2}+1....,{N\over2}-1\}}
\\
k_z={2\pi m\over L}, \text n\in{\{-{N\over2},-{N\over2}+1....,{N\over2}-1\}}
$$

故：
$$
h(x, z, t)=\sum_{m=-{N\over2}}^{{N\over 2}-1}\sum_{n=-{N\over2}}^{{N\over 2}-1}\widetilde h({2\pi n\over L}, {2\pi m\over L}, t)e^{i({2\pi n\over L}x+{2\pi m\over L}z)}
$$
为使下标从0开始，令m'=m+N/2, n'=n+N/2，则n', m'∈{0,1,...,N-1}
$$
h(x, z, t)=\sum_{m'=0}^{N-1}\sum_{n'=0}^{N-1}\widetilde h({2\pi (n'- {N\over 2})\over L}, {2\pi (m'- {N\over 2})\over L}, t)e^{i({2\pi n\over L}x+{2\pi m\over L}z)}
$$

$$
令h(n', m', t)=\widetilde h({{2\pi(n'-{N\over 2})}\over L},{{2\pi(m'-{N\over 2})}\over L},t)
\\
将e^{i{{2\pi(m'-{N\over 2})z}\over L}}从内层求和号中提出，得：
\\
h(x, z, t)=\sum_{m'=0}^{N-1}e^{i{{2\pi(m'-{N\over 2})z}\over L}}
\sum_{m'=0}^{N-1}\widetilde h'(n',m',t)e^{i{2\pi(n'-{N\over 2})x\over L}}
$$

上式可拆为：
$$
h(x,z,t)=\sum_{m'=0}^{N-1}h''(x,m',t)e^{i{2\pi(m'-{N\over 2})z\over L}}...(a)
\\
\widetilde h''(x,m',t)=\sum_{n'=0}^{N-1}\widetilde h'(n',m',t)e^{i{{2\pi(n'-{N\over 2})x}\over L}}...(b)
$$
L长度任意，于是取L=N，并展开x和z
$$
x={uL\over N} \text ,u\in{\{{-N\over 2},{-N\over 2}+1,...,{N\over 2}-1\}}
\\
z={vL\over N} \text ,u\in{\{{-N\over 2},{-N\over 2}+1,...,{N\over 2}-1\}}
\\
因为L=N
\\
x=u' - {N\over 2} \text ,u'\in{\{0,1,...,N-1\}}
\\
z=v' - {N\over 2} \text ,v'\in{\{0,1,...,N-1\}}
$$
![image text](https://github.com/Telluluu/QGStudioSummerLearning/blob/main/Week3/FFT%E5%85%AC%E5%BC%8F%E6%8E%A8%E5%AF%BC.png)
