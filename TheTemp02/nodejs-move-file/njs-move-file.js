const fs = require('fs');
const fsp = require('fs/promises'); 
const path = require('path');

async function main(){
	
	    // 解析为绝对路径，避免相对路径混乱
    const configPth = path.resolve(__dirname, "./move_file_config.json");
	console.log("configPth = " + configPth)
    if (!fs.existsSync(configPth)){
		console.log("move_file_config.json 文件不存在")
	}
		
    const fileContent = fs.readFileSync(configPth, 'utf8');	

	console.log("fileContent = " + fileContent)
	
	const jsonObj = JSON.parse(fileContent);
	console.log('同步读取成功：', jsonObj);

	jsonObj.forEach((item) => {
		
		if (!fs.existsSync(item.from_pth))
			return;
		
				// 确定 JSON 输出路径
		const outputPath = path.resolve(__dirname, item.to_pth) // 自定义输出目录
		
		
		const content = fs.readFileSync(item.from_pth, 'utf8');	

		fs.writeFile(outputPath, content, 'utf8', (err) => {
				if (err) {
					console.error('写入文件失败:', err);
					return;
				}
				console.log(`xmlContent 文件已保存到: ${outputPath}`);
			});
	})
	
}






main();









