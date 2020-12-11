
<?php

$dbname = $_POST['dbnm'];
$sql = $_POST['sql'];

$envfilename = "../env/dbenv.txt";
$fp = fopen($envfilename, 'r');
fgets($fp);
$str = fgets($fp); $ary = explode(",", $str); $server = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $username = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $password = $ary[0];
fclose($fp);

if($mysql = mysql_pconnect($server,$username,$password)){
	mysql_select_db($dbname, $mysql);
	$mysql_result = mysql_query($sql, $mysql);
	mysql_close($mysql);
	if($mysql_result == FALSE){
		$str1 = "0,3,";
		echo $str1;
		return;
	}
	$str1 = "1.0";
}else{
	$str1 = "0,1,,";
}
echo $str1;
?> 
