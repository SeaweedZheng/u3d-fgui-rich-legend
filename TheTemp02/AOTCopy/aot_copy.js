const fs = require('fs').promises;
const path = require('path');

async function renameAndMoveFiles(sourceDir, targetDir) {
    try {
        // 定义需要处理的文件列表
        const lst = [
		"Newtonsoft.Json.dll",
		"SelfAOT.dll",
		"System.Core.dll",
		"System.dll",
		"UnityEngine.AndroidJNIModule.dll",
		"UnityEngine.AssetBundleModule.dll",
		"UnityEngine.CoreModule.dll",
		"UnityEngine.JSONSerializeModule.dll",
		"mscorlib.dll",
        ];
        
        // 确保目标目录B存在
        await fs.mkdir(targetDir, { recursive: true });
        
        // 遍历处理lst中的每个文件
        for (const file of lst) {
            const sourceFilePath = path.join(sourceDir, file);
            const targetFileName = `${file}.bytes`;
            const targetFilePath = path.join(targetDir, targetFileName);
            
            // 检查文件是否存在
            try {
				console.log("目标文件: ",sourceFilePath);
                await fs.access(sourceFilePath);
                
                // 读取源文件内容
                const fileContent = await fs.readFile(sourceFilePath);
                
                // 写入到目标文件（覆盖式）
                await fs.writeFile(targetFilePath, fileContent);
                
                console.log(`已将 ${file} 重命名为 ${targetFileName} 并移动到 ${targetDir}`);
            } catch (error) {
                console.warn(`文件 ${file} 不存在于源目录中，跳过处理`);
            }
        }
        
        console.log('所有指定文件处理完成');
    } catch (error) {
        console.error('处理文件时出错:', error);
    }
}

// 使用示例
//const sourceDirectory = 'E:/work4/SBoxTest/HybridCLRData/AssembliesPostIl2CppStrip/Android'; // 替换为实际的源目录路径
//const targetDirectory = 'E:/work4/SBoxTest/TheOutput/AOTCopy/AOT'; // 替换为实际的目标目录路径

const sourceDirectory = '../../HybridCLRData/AssembliesPostIl2CppStrip/Android'; // 可以是相对路径或绝对路径
const targetDirectory = './AOT'; // 可以是相对路径或绝对路径


renameAndMoveFiles(sourceDirectory, targetDirectory);  



