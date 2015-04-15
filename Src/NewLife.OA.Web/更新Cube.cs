// 自动选择最新的文件源
var src = @"C:\X\NewLife.Cube".AsDirectory();
var src2 = @"E:\X\NewLife.Cube".AsDirectory();
if(src.LastWriteTime < src2.LastWriteTime) src = src2;

var di = ".".AsDirectory();
var root = di.FullName.EnsureEnd("\\");

Console.WriteLine("复制 {0} => {1}", src.FullName, root);

foreach(var fi in di.GetAllFiles("*.cshtml"))
{
	var fileName = fi.FullName.TrimStart(root);
	//Console.WriteLine(fileName);
	var srcFile = src.FullName.CombinePath(fileName).AsFile();
	// 源文件必须存在，并且比当前文件要新
	if(!srcFile.Exists || srcFile.LastWriteTime <= fi.LastWriteTime) continue;
	
	try
	{
		Console.Write(fileName);
	    srcFile.CopyTo(fi.FullName, true);
		Console.WriteLine("\tOK!");
	}
	catch(Exception ex) { Console.WriteLine(" " + ex.Message); }
}
