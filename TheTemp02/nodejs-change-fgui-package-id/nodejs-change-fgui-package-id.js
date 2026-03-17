const fs = require('fs');
const fsp = require('fs/promises'); 
const path = require('path');



// ==================== 脚本执行入口 ====================
// 请修改这里为你要处理的文件夹路径（绝对路径/相对路径均可）
//const targetDir = './target-folder'; 
// 【这个路劲反斜杠写法有问题】： const targetDir = 'E:\work\u3d-fgui-coin-pusher-rich-legend-2001003\TheSourceFile\fgui-coin-pusher-rich-lengend-2001003-3'; 
const targetDir = 'E:\\work\\u3d-fgui-coin-pusher-rich-legend-2001003\\TheSourceFile\\fgui-coin-pusher-rich-lengend-2001003-3';

async function main(){
	
	    // 解析为绝对路径，避免相对路径混乱
    const configPth = path.resolve(__dirname, "./package_id_change_config.json");
	console.log("configPth = " + configPth)
    if (!fs.existsSync(configPth)){
		console.log("package_id_change_config.json 文件不存在")
	}
		
    const fileContent = fs.readFileSync(configPth, 'utf8');	

	console.log("fileContent = " + fileContent)
	
	const jsonObj = JSON.parse(fileContent);
	console.log('同步读取成功：', jsonObj);


  // 调用遍历函数，在回调中处理文件
  await traverseDirectory(path.resolve(targetDir), ['.xml', '.json', '.info'], async (filePaths) => {

	//console.log("filePaths = ",filePaths)

    // 遍历收集到的路径，逐个处理文件
    for (const filePath of filePaths) {
      await replaceContentInFile(filePath, jsonObj);
    }
	
  });
	
}


/**
 * 替换文件内容的核心函数
 * @param {string} filePath - 文件完整路径
 */
async function replaceContentInFile(filePath ,jsonObj) {
  try {
    // 读取文件内容（使用 utf8 编码确保正确处理文本文件）
    let content = await fsp.readFile(filePath, 'utf8');
    
	let isChange = false;
	jsonObj.forEach((item) => {
		
		if (content.includes(item.from_id)) {
			isChange = true;
			content = content.replaceAll(item.from_id, item.to_id);
		}
	})
	
	if(isChange){
		
		// 写入替换后的内容（覆盖原文件）
		await fsp.writeFile(filePath, content, 'utf8');
		
		console.log(`已处理文件: ${filePath}`);
	}else{
		
		// console.log(`文件无匹配内容，跳过: ${filePath}`);
	}
	
  } catch (error) {
    console.error(`处理文件失败: ${filePath}`, error.message);
  }
}



/**
 * 递归遍历文件夹，收集所有目标后缀的文件路径
 * @param {string} dirPath - 文件夹路径
 * @param {function} callback - 回调函数，参数为收集到的文件路径数组
 */
async function traverseDirectory(dirPath,  targetExtensions =['.xml', '.json', '.info'], callback = null) {
  // 用于存储所有符合条件的文件路径
  const targetFilePaths = [];
  
  try {
    // 读取文件夹中的所有文件/子文件夹
    const files = await fsp.readdir(dirPath, { withFileTypes: true });
    
    // 定义需要处理的文件后缀
    //const targetExtensions = ['.xml', '.json', '.info'];
    
    for (const file of files) {
      const fullPath = path.join(dirPath, file.name);
      
      if (file.isDirectory()) {
        // 如果是文件夹，递归处理，收集子文件夹中的路径
        const subDirPaths = await traverseDirectory(fullPath, targetExtensions, () => {}); // 递归时暂不触发回调
        targetFilePaths.push(...subDirPaths); // 合并子文件夹的路径
      } else if (file.isFile()) {
        // 获取文件后缀（转小写，避免大小写问题）
        const ext = path.extname(file.name).toLowerCase();
        // 检查是否是目标后缀的文件
        if (targetExtensions.includes(ext)) {
          targetFilePaths.push(fullPath); // 收集符合条件的文件路径
        }
      }
    }
    
    // 触发回调，返回收集到的所有路径
    if (typeof callback === 'function') {
      callback(targetFilePaths);
    }
    
    // 返回路径数组（供递归调用时合并路径）
    return targetFilePaths;
    
  } catch (error) {
    console.error(`遍历文件夹失败: ${dirPath};`, error.message);
    // 即使出错，也触发回调（返回空数组），避免后续逻辑中断
    if (typeof callback === 'function') {
      callback([]);
    }
    return [];
  }
}






















main();









