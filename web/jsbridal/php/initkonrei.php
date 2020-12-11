<?php

//$dbname = "bridal";
//$konreitable = "bridaluser";
//$krno = "012952";
//$krpw = "429866";

$dbname = $_POST['dbnm'];
$konreitable = $_POST['krtbl'];
$krno = $_POST['krno'];
$krpw = $_POST['krpw'];
$gesttable = "ge".$krno;

$envfilename = "../env/dbenv.txt";
$fp = fopen($envfilename, 'r');
fgets($fp);
$str = fgets($fp); $ary = explode(",", $str); $server = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $username = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $password = $ary[0];
fclose($fp);

if($mysql = mysql_connect($server,$username,$password)){
	mysql_select_db($dbname, $mysql);

	$sql = "SELECT * FROM ".$konreitable." WHERE (userno=".$krno.");";
	$rows = mysql_query($sql, $mysql);
	$row = mysql_fetch_array($rows);
	if($row != NULL){
		mysql_close($mysql);
		echo "1,".$row[id].",";
		return;
	}
	$sql = "INSERT INTO ".$konreitable."";
	$sql = $sql." (userno,username,password)";
	$sql = $sql." VALUES (".$krno.",'".$krno."','".$krpw."');";
	$ret = mysql_query($sql, $mysql);
	if($ret == FALSE){
		mysql_close($mysql);
		echo "0,2";
		return;
	}
	$id = mysql_insert_id($mysql);

	$sql = "CREATE TABLE ".$gesttable."";
	$sql = $sql." (id INT NOT NULL AUTO_INCREMENT PRIMARY KEY";
	$sql = $sql.",flag INT NOT NULL,tno INT NOT NULL,sno INT NOT NULL";
	$sql = $sql.",name1 VARCHAR(24),name2 VARCHAR(24),yomi VARCHAR(48)";
	$sql = $sql.",sama VARCHAR(15),kind VARCHAR(30),skind VARCHAR(30)";
	$sql = $sql.",adrsno VARCHAR(24),adrs1 VARCHAR(90),adrs2 VARCHAR(90)";
	$sql = $sql.",adrs3 VARCHAR(90),telno VARCHAR(24),kt1 VARCHAR(60)";
	$sql = $sql.",kt2 VARCHAR(60),kt3 VARCHAR(60),kt4 VARCHAR(60)";
	$sql = $sql.",gno INT NOT NULL,sub1 VARCHAR(96),sub2 VARCHAR(96)";
	$sql = $sql.",gift VARCHAR(30),dish VARCHAR(30));";
	$ret = mysql_query($sql, $mysql);
	if($ret == FALSE){
		mysql_close($mysql);
		echo "0,3";
		return;
	}
	mysql_close($mysql);

	$folder = "../list/".$gesttable;
	if(!file_exists($folder)){
		mkdir($folder, 0777);
	}
	$retstr = "1,".$id.",";
}else{
	$retstr = "0,1,";
}
echo $retstr;

?>
