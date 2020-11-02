<?php   
$argvs = $_POST['com'];
mb_convert_variables('SJIS', 'UTF-8', $argvs);
error_reporting(0);
$fp = fopen("list/user.txt", 'w');
$retstr = "0,";
if($fp != 0){
	$ret=fwrite($fp,$argvs);
	fclose($fp);
}
echo $ret;
?> 
