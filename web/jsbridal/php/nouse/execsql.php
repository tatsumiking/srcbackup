<?php
$dbname = $_POST['com'];
$sqlstr = $_POST['sql'];
$filds = $_POST['fld'];
$key = explode(",", $filds);
$max = count($key);

$filename = "env/dbenv.txt";
$fp = fopen($filename, 'r');
fgets($fp); // ƒRƒƒ“ƒgs
$server = fgets($fp);$server = substr($server, 0, -2);
$username = fgets($fp);$username = substr($username, 0, -2);
$password = fgets($fp);$password = substr($password, 0, -2);
fclose($fp);

$retstr="stat=1.0&list=";
if($mysql = mysql_pconnect($server,$username,$password)){
	mysql_select_db($dbname, $mysql);
	$rows = mysql_query($sqlstr, $mysql);
	while(1){
		$row = mysql_fetch_array($rows);
		if($row == NULL){
			break;
		}
		$recode = "":
		for($idx = 0; $idx < $max; $idx++){
			$recode = $recode.$row[$key[$idx]]." ";
		}
		$retstr = $retstr.$recode."|";
	}
	mysql_close($mysql);
}
echo $retstr;
?> 
