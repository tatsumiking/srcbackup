<?php
$dbname = $_POST['dbnm'];
$tblname = $_POST['tble'];
$fildstr = $_POST['fild'];
$sortsql = $_POST['sort'];
//$dbname = "bridal";
//$tblname = "kaijyou";
//$fildstr = "id,name";
$a = explode(",", $fildstr);

$envfilename = "../env/dbenv.txt";
$fp = fopen($envfilename, 'r');
fgets($fp);
$str = fgets($fp); $ary = explode(",", $str); $server = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $username = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $password = $ary[0];
fclose($fp);

$sql = "SELECT * FROM ".$tblname.$sortsql.";";
if($mysql = mysql_pconnect($server,$username,$password)){
	mysql_select_db($dbname, $mysql);
	$rows = mysql_query($sql, $mysql);
	mysql_close($mysql);
	if($rows == NULL){
		$retstr = "0,2,"."<".$dbname.">";
		echo $retstr;
		return;
	}
	$retstr = "";
	while(1){
		$row = mysql_fetch_array($rows);
		if($row == NULL){
			break;
		}
		$max = count($a);
		$retstr = $retstr.$row[$a[0]];
		for($i = 1; $i < $max; $i++){
			if($a[$i] == ""){
				$retstr = $retstr.",0";
			}else{
				$retstr = $retstr.",".$row[$a[$i]];
			}
		}
		$retstr = $retstr.";";
	}
}else{
	$retstr = "0,1,,";
	echo $retstr."<".$dbname.">";
	return;
}
//mb_convert_variables('sjis-win', 'UTF-8', $retstr);
echo $retstr;
?> 
