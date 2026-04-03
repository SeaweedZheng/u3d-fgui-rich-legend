# 修改时间： 20250320

# ctrl
* == View + Presenter
* 拥有ui对象
* 负责对外提供事件接口
* 不提供协程（方便后期改为异步也可用）？？
* 滚轮流程控制？
* 发送事件


# view
* 拥有ui对象
* 提供tween（可跟换过为DoTween）
* 提供协程或异步

# presenter
* 拥有IV对象