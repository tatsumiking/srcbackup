
<?php
//$dbname = "bridal";
//$konreitable = "bridaluser";
//$recid = 7;
//$konreino = "012950";

$dbname = $_POST['dbnm'];
$konreitable = $_POST['krtbl'];
$recid = $_POST['recid'];
$konreino = $_POST['krno'];

$gesttable = "ge".$konreino;

$envfilename = "../env/dbenv.txt";
$fp = fopen($envfilename, 'r');
fgets($fp);
$str = fgets($fp); $ary = explode(",", $str); $server = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $username = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $password = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $saveurl = $ary[0];
fclose($fp);

$csvfilename = "../list/ge".$konreino."/ge".$konreino.".csv";
if($mysql = mysql_pconnect($server,$username,$password)){
	mysql_select_db($dbname, $mysql);

	$fp = fopen($csvfilename, 'r');
	if($fp == FALSE){
		mysql_close($mysql);
		$retstr = "0,2,";
		echo $retstr;
		return;
	}

	$sql = "DROP TABLE ".$gesttable.";";
// echo $sql;
	$ret = mysql_query($sql, $mysql);

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
// echo $sql;
	$ret = mysql_query($sql, $mysql);
	if($ret == FALSE){
		mysql_close($mysql);
		fclose($fp);
		$retstr = "0,4,";
		echo $retstr;
		return;
	}

	$str = fgets($fp);// 両家情報スキップ
	$str = fgets($fp);// 基本情報スキップ
	$gestcount = 0;
	while(1){
		if(feof($fp)){
			break;
		}
		$str = fgets($fp);
		$str = mb_convert_encoding($str, 'UTF-8', 'sjis-win');
		$a = split(',',$str);
		$len = strlen($a[2]); $len = $len-2; $a[2] = substr($a[2], 1, $len);
		$len = strlen($a[3]); $len = $len-2; $a[3] = substr($a[3], 1, $len);
		$len = strlen($a[4]); $len = $len-2; $a[4] = substr($a[4], 1, $len);
		$len = strlen($a[5]); $len = $len-2; $a[5] = substr($a[5], 1, $len);
		$len = strlen($a[6]); $len = $len-2; $a[6] = substr($a[6], 1, $len);
		$len = strlen($a[7]); $len = $len-2; $a[7] = substr($a[7], 1, $len);
		$len = strlen($a[8]); $len = $len-2; $a[8] = substr($a[8], 1, $len);
		$len = strlen($a[9]); $len = $len-2; $a[9] = substr($a[9], 1, $len);
		$len = strlen($a[10]); $len = $len-2; $a[10] = substr($a[10], 1, $len);
		$len = strlen($a[11]); $len = $len-2; $a[11] = substr($a[11], 1, $len);
		$len = strlen($a[12]); $len = $len-2; $a[12] = substr($a[12], 1, $len);
		$len = strlen($a[13]); $len = $len-2; $a[13] = substr($a[13], 1, $len);
		$len = strlen($a[14]); $len = $len-2; $a[14] = substr($a[14], 1, $len);
		$len = strlen($a[15]); $len = $len-2; $a[15] = substr($a[15], 1, $len);
		$len = strlen($a[16]); $len = $len-2; $a[16] = substr($a[16], 1, $len);
		if($a[2] != ""){
			$kubun = substr($a[13],0,4);
			if($kubun == "新郎"){
				$flag = 1;
			}else if($kubun == "新婦"){
				$flag = 2;
			}else{
				$flag = 0;
			}
			if($gestcount == 0){
				$datasql = "";
			}else{
				$datasql=$datasql.",";
			}
			$datasql=$datasql."('".$flag."','',''";// tno,sno
			$datasql=$datasql.",'".$a[2]."','".$a[3]."','".$a[4]."','".$a[5]."'";
			$datasql=$datasql.",'".$a[6]."','".$a[7]."','".$a[8]."','".$a[9]."'";
			$datasql=$datasql.",'".$a[10]."','".$a[11]."','".$a[12]."'";
			$datasql=$datasql.",'".$a[13]."','".$a[14]."','".$a[15]."','".$a[16]."'";
			$datasql=$datasql.",'','','','',''";
			$datasql=$datasql.")";
			$gestcount++;
		}
	}
	$sql="INSERT INTO ".$gesttable." (";
	$sql=$sql."flag,tno,sno";
	$sql=$sql.",name1,name2,yomi,sama";
	$sql=$sql.",kind,skind,adrsno,adrs1";
	$sql=$sql.",adrs2,adrs3,telno";
	$sql=$sql.",kt1,kt2,kt3,kt4";
	$sql=$sql.",gno,sub1,sub2,gift,dish)";
	$sql=$sql." VALUES ".$datasql.";";
//echo $sql;
	$ret = mysql_query($sql, $mysql);
	if($ret == FALSE){
		mysql_close($mysql);
		fclose($fp);
		$retstr = "0,5,";
		echo $retstr;
		return;
	}
	fclose($fp);
	mysql_close($mysql);
	$retstr = "1,".$gestcount.",";
}else{
	$retstr = "0,1,";
}
echo $retstr;
?>

