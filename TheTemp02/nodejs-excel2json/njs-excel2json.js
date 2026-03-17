const fs = require('fs');
const fsp = require('fs/promises'); 
const path = require('path');
const XLSX = require('xlsx');


let excelFolderDir = "./TheExcel"
let outputFolderDir = "./TheOutput"



/**
 * 筛选 Excel 文件（判断后缀名）
 * @param {string} filename - 文件名
 * @returns {boolean} 是否为 Excel 文件
 */
function isExcelFile(filename) {
  const excelExtensions = ['.xlsx', '.xls', '.xlsm']; // 常见 Excel 格式
  const ext = path.extname(filename).toLowerCase(); // 获取文件后缀（小写统一判断）
  return excelExtensions.includes(ext);
}



/**
 * 遍历文件夹，获取所有 Excel 文件的绝对路径
 * @param {string} folderPath - 目标文件夹路径（相对/绝对）
 * @param {object} options - 遍历配置
 * @param {boolean} options.recursive - 是否递归遍历子文件夹（默认 true）
 * @returns {string[]} 所有 Excel 文件的绝对路径数组
 */
async function getExcelFilesInFolder(folderPath, options = {}) {
  const { recursive = true } = options;
  const excelFiles = [];

  try {
    // 解析为绝对路径，避免相对路径混乱
    const absoluteFolderPath = path.resolve(__dirname, folderPath);
	console.log("absoluteFolderPath = ",absoluteFolderPath)

    // 检查文件夹是否存在
    try {
      await fsp.access(absoluteFolderPath, fsp.constants.F_OK);
    } catch {
      throw new Error(`文件夹不存在：${absoluteFolderPath}`);
    }

    // 遍历文件夹
    const traverse = async (currentDir) => {
      const files = await fsp.readdir(currentDir, { withFileTypes: true }); // withFileTypes 让返回值包含文件类型信息

      for (const file of files) {
        const filePath = path.join(currentDir, file.name);

        if (file.isDirectory() && recursive) {
          // 如果是文件夹且开启递归，继续遍历子文件夹
          await traverse(filePath);
        } else if (file.isFile() && isExcelFile(file.name)) {
          // 如果是文件且为 Excel 格式，加入数组
          excelFiles.push(filePath);
        }
      }
    };

    await traverse(absoluteFolderPath);

    return excelFiles;
  } catch (error) {
    console.error('文件夹遍历失败：', error.message);
    //process.exit(1);
  }
}





function getFirstSheetFirstRow(excelPath, keyRowIndex, typeRowIndex) {
  try {
    const workbook = XLSX.readFile(excelPath);
    // 获取第一个工作表名称（workbook.SheetNames[0] 是第一个工作表）
    const firstSheetName = workbook.SheetNames[0];
    if (!firstSheetName) {
      throw new Error('Excel 文件中无工作表');
    }

    const worksheet = workbook.Sheets[firstSheetName];
    // 转为二维数组，仅取第一行（索引 0）
    const sheetData = XLSX.utils.sheet_to_json(worksheet, {
      header: 1, // 整张表，输出二维数组 [[row1], [row2]]  //[row][col]  
      raw: false, // 转为字符串，保持原始显示
      defval: '' // 空单元格填充为空字符串
    });

    //const firstRow = sheetData.length > 0 ? sheetData[0] : [];


	const keyRow  = sheetData.length > keyRowIndex ? sheetData[keyRowIndex] : [];
	
	const typeRow = sheetData.length > typeRowIndex ? sheetData[typeRowIndex] : [];
	

    // 控制台输出结果
    console.log(`📄 文件：${path.basename(excelPath)}`);
    console.log(`  第一个工作表：${firstSheetName}`);
	console.log(`  第二个工作表：${workbook.SheetNames[1]}`);
    console.log(`  第一行数据：${JSON.stringify(keyRow, null, 2)}`);
    console.log('----------------------------------------');

    return {
      //filename: path.basename(excelPath),  // 包含后缀名
	  filename: path.basename(excelPath, path.extname(excelPath)), // 去掉后缀名
      sheetName: firstSheetName,
      keyRow: keyRow,
	  typeRow: typeRow
    };
  } catch (error) {
    console.error(`读取 ${path.basename(excelPath)} 失败：`, error.message);
    console.log('----------------------------------------');
    return null;
  }
}




async function main(){
	

	const excelDir = path.join(__dirname, excelFolderDir);
	if (!fs.existsSync(excelDir)) {
		fs.mkdirSync(excelDir, { recursive: true }); // recursive: true 确保多层目录创建
	}
			
	const outputDir = path.join(__dirname, outputFolderDir);
	if (!fs.existsSync(outputDir)) {
		fs.mkdirSync(outputDir, { recursive: true }); // recursive: true 确保多层目录创建
	}

	const excelFiles = await getExcelFilesInFolder(excelDir, { recursive: true  });
	console.log("excelFiles = ",excelFiles);
	if (excelFiles.length === 0) {
		console.log('未找到任何 Excel 文件，程序退出');
		return;
	}
	
	/*

	return {
      filename: path.basename(excelPath),
      sheetName: firstSheetName,
      keyRow: keyRow,
	  typeRow: typeRow
    };
	*/
	


	excelFiles.forEach((excelPath) => {
		let info = getFirstSheetFirstRow(excelPath,0,2);
		
		let keyNames = {}
		let valueTypes = {}
		let idCol = null
		let idKey = null		
		
		info.keyRow.forEach((v,index)=>{
			if(!v.startsWith("#") && v != ""){
				
				keyNames[index]	= v	
				
				if(v == "id")
					idCol = index
				
				if(v == "key")
					idKey = index	

			}				
		})
		info.typeRow.forEach((v,index)=>{
			if(!v.startsWith("#") && v != "")
				valueTypes[index] = v		
		})
		
		
		let  excelFilename = info.filename;	
		
		
		const workbook = XLSX.readFile(excelPath);
		// 获取第一个工作表名称（workbook.SheetNames[0] 是第一个工作表）
		const firstSheetName = workbook.SheetNames[0];
		if (!firstSheetName) {
		  throw new Error('Excel 文件中无工作表');
		}

		const worksheet = workbook.Sheets[firstSheetName];
		// 转为二维数组，仅取第一行（索引 0）
		const sheetData = XLSX.utils.sheet_to_json(worksheet, {
		  header: 1, // 整张表，输出二维数组 [[row1], [row2]]  //[row][col]  
		  raw: false, // 转为字符串，保持原始显示
		  defval: '' // 空单元格填充为空字符串
		});
		

		
		
		if(sheetData.length == 0) return;
		
		let jsonResult = [];
		
		sheetData.forEach((row) => {
			if(row.length == 0)return;
			
			if(row[0].startsWith("#") ) return;
			
			if(row[idCol] == "") {
				//console.error(`第${excelFilename}行没有填写id`);
				return;
			}
			
			if(idKey != null && row[idKey] == "") {
				//console.error(`第${excelFilename}行没有填写id`);
				return;
			}		
			
			let data = {}
			row.forEach((val,index) => {
				if(index in keyNames){
					
					let v = val;
					
					if(index in valueTypes){
						switch(valueTypes[index]){
							case "string":
								{
									v = val.toString();
								}
								break;
							case "int":
							case "float":
								{
									try{
										v = Number(val);
									}
									catch(e){
										v = val;
									}
								}
								break;
							case "bool":
								{
									let trimmedVal = (val ?? '').toString().trim().toLowerCase();
									if (trimmedVal === 'true' || trimmedVal === '1') {
										v = true;
									} else if (trimmedVal === 'false' || trimmedVal === '0' || trimmedVal === '') {
										v = false;
									}
								}
								break;
						}
					}

					data[keyNames[index]] = v
				}
			})
			jsonResult.push(data);
		})

		
		// 确定 JSON 输出路径
		const outputPath = path.resolve(outputDir, `${excelFilename}.json`) // 自定义输出目录

		fs.writeFile(outputPath, JSON.stringify(jsonResult, null, 2), 'utf8', (err) => {
				if (err) {
					console.error('写入文件失败:', err);
					return;
				}
				console.log(`xmlContent 文件已保存到: ${outputPath}`);
			});
	})
}






main();









