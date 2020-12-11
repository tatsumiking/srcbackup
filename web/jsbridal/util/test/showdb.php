<?php

//$dbname = $_POST['dbnm'];
$dbname = "hotelsoa";
$filename = "../env/dbenv.txt";

$fp = fopen($filename, 'r');

fgets($fp); // コメントスキップ
$str = fgets($fp); $ary = explode(",", $str); $server = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $username = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $password = $ary[0];
fclose($fp);

if($mysql = mysql_connect($server,$username,$password)){
	$sql = "SHOW DATABASES;";
	$rows = mysql_query($sql, $mysql);
	if($rows == null){
		$str1 = "comp=0,2,";
		echo $str1;
		return;
	}
	while(1){
		$row = mysql_fetch_array($rows);
		if($row == NULL){
			break;
		}
		$max = count($row);
		$str = "[".$row[0]."]";
		for($idx = 1; $idx < $max; $idx++){
			$str = $str."[".$row[$idx]."]";
		}
		echo $str."<BR>";
	}
	mysql_close($mysql);
}else{
	echo "0,1,接続エラー"."<BR>";
	return;
}

$str = "1,".$dbname.",";
echo $str;

?>

