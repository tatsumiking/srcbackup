<?php

$argvs = $_POST['com'];
$argvs = mb_convert_encoding($argvs, 'SJIS', 'UTF-8');
$a = explode(",", $argvs);

$filename = "../env/dbenv.txt";
$fp = fopen($filename, 'r');
$str = fgets($fp); $ary = explode(",", $str); $server = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $username = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $password = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $dbname = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $tblname = $ary[0];
fclose($fp);

$num = intval($a[0]);
$tblname = "kihn".sprintf('%04d', $num);

$sql = "CREATE TABLE `".$tblname."` ";
$sql = $sql."(`id` INT NOT NULL AUTO_INCREMENT PRIMARY KEY";
$sql = $sql.",`userno` INT NOT NULL";
$sql = $sql.",`kyosiki` VARCHAR(32),`hirouen` VARCHAR(96)";
$sql = $sql.",`adrsno` VARCHAR(16)";
$sql = $sql.",`adrs` VARCHAR(90),`adrs2` VARCHAR(90)";
$sql = $sql.",`tel` VARCHAR(16),`tel2` VARCHAR(16)";
$sql = $sql.",`fax` VARCHAR(16),`email` VARCHAR(96))";

echo $sql."</BR>";
if($mysql = mysql_connect($server,$username,$password)){
	echo "<BR>connect sccess<BR>";
	$mysql_result = mysql_select_db($dbname, $mysql);
	var_dump($mysql_result); echo "</BR>";
	$mysql_result = mysql_query($sql, $mysql);
	var_dump($mysql_result); echo "</BR>";
	$mysql_result = mysql_close($mysql);
	var_dump($mysql_result); echo "</BR>";
}else{
	echo "<BR>connect fail<BR>";
	return;
}

$str1 = "comp=".$dbname.",".$tblname.",";
echo $str1;

?>

