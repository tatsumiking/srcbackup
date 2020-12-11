<?php   
$filename = $_POST['file'];
error_reporting(0);
$fp = fopen($filename, 'r');
$retstr = "0,";
if($fp != 0){
	$retstr=fread($fp,1048576);
	fclose($fp);
	$retstr = str_replace("\r\n", "\n", $retstr);
	$retstr = str_replace("\n", "\r\n", $retstr);
	mb_convert_variables('UTF-8', 'SJIS', $retstr);
}
echo $retstr;
?> 
