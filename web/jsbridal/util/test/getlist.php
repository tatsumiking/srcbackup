<?php
//$dbname = $_POST['dbnm'];
//$tblname = $_POST['tble'];
//$fildstr = $_POST['fild'];
$dbname = "hotelsoa";
$tblname = "kaijyou";
$fildstr = "id,name";
$a = explode(",", $fildstr);

$envfilename = "../env/dbenv.txt";
$fp = fopen($envfilename, 'r');
fgets($fp);
$str = fgets($fp); $ary = explode(",", $str); $server = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $username = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $password = $ary[0];
fclose($fp);

$sql = "SELECT * FROM ".$tblname.";";
if($mysql = mysql_pconnect($server,$username,$password)){
	mysql_select_db($dbname, $mysql);
	$rows = mysql_query($sql, $mysql);
	mysql_close($mysql);
	if($rows == NULL){
		$str1 = "comp=0,2,";
		echo $str1;
		return;
	}
	$str1 = "";
	while(1){
		$row = mysql_fetch_array($rows);
		if($row == NULL){
			break;
		}
		echo "[".$row."]\r\n";
		$max = count($a);
		$str1 = $str1.$row[$a[0]];
		for($i = 1; $i < $max; $i++){
			if($a[$i] == ""){
				$str1 = $str1.",0";
			}else{
				$str1 = $str1.",".$row[$a[$i]];
			}
		}
		$str1 = $str1.";";
	}
}else{
	$str1 = "comp=0,1,,";
	echo $str1;
	return;
}
//mb_convert_variables('sjis-win', 'UTF-8', $str1);
echo $str1;
?> 
