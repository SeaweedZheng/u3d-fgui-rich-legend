# 概要

* 复制某个fgui项目进行换皮，但是在多合一平台运行时，发现游戏的fgui包id一样导致冲突！

* 【任务】重置项目的包id！



# 包id

* ABC包，在fgui项目里所在路劲（文件夹）： fgui项目根路径/assets/ABC/

* ABC包id所在位置： fgui项目根路径/assets/ABC/package.xml   里的 “<packageDescription id="adoe8w7d">”  id进行修改！


# 修改包id需要修改的所有文件：
* fgui项目根路径/assets/XXX包名/package.xml   里的 “<packageDescription id="adoe8w7d">”  id进行修改！
* fgui项目根路径/.objs/workspace.json  adoe8w7d 进行修改
* 项目所有loader.url 带有 adoe8w7d 的进行修改该。（如：ui://adoe8w7dj4ebc）  搜索“ui://adoe8w7d”

写个脚本，遍历所有.xml和.json文件 .info(.objs文件夹中的)
将adoe8w7d 该为  adoe8w7e
ui://adoe8w7d？？？ 该为  ui://adoe8w7e？？？

# 创建工程
* cd到工程目录
* npm init 



# 插件安装：

npm install xlsx --save