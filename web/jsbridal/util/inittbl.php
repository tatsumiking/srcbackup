<?php

$dbname = $_POST['dbnm'];
$tblname = $_POST['tble'];
$fildstr = $_POST['fild'];
$typestr = $_POST['type'];
$filds = explode(",", $fildstr);
$types = explode(",", $typestr);

$filename = "../env/dbenv.txt";
$fp = fopen($filename, 'r');
fgets($fp); // コメントスキップ
$str = fgets($fp); $ary = explode(",", $str); $server = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $username = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $password = $ary[0];
fclose($fp);

$sql = "CREATE TABLE `".$tblname."` ";
$sql = $sql."(`id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY";
$max = $filds.length;
for($idx = 0; $idx < $max; $idx++){
	$num = intval($types[$idx]);
	if($num == 0){
		$sql = $sql.",".$filds[$idx]." INT NOT NULL";
	}else{
		$sql = $sql.",".$filds[$idx]." VARCHAR(".$num.")";
	}
}
$sql = $sql.");";

if($mysql = mysql_connect($server,$username,$password)){
	$ret = mysql_select_db($dbname, $mysql);
	if($ret == FALSE){
		echo "0,2";
		return;
	}
	$ret = mysql_query($sql, $mysql);
	if($ret == FALSE){
		echo "0,3";
		return;
	}
	$ret = mysql_close($mysql);
	if($ret == FALSE){
		echo "0,4";
		return;
	}
}else{
	echo "0,1";
	return;
}

$str = "1.0,".$dbname.",".$tblname.",";
echo $str;

?>

