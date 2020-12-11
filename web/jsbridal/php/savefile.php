<?php   
$filename = $_POST['file'];
$data = $_POST['data'];

mb_convert_variables('SJIS', 'UTF-8', $filename);
error_reporting(0);
$fp = fopen($filename, 'w');
$retstr = "0,1";
if($fp == 0){
	echo $retstr;
	return;
}
$len=fwrite($fp,$data);
fclose($fp);
$retstr = "1,".$len.",";
echo $retstr;
?> 
