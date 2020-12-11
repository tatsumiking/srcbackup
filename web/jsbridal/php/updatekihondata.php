<?php
//$datas = "2,000001,2020-12-01 13:00:00,2020-12-01 14:00:00,�P���̊�,0,���j,����,����,����,���,���b,,,,,";
//mb_convert_variables('UTF-8', 'SJIS', $argvs);
$dbname = $_POST['dbnm'];
$recid = $_POST['recid'];
$datas = $_POST['data'];
$a = explode(",", $datas);

// �T�[�o�[�����i�[�t�@�C���쐬
$envfilename = "../env/dbenv.txt";
$fp = fopen($envfilename, 'r');
fgets($fp);
$str = fgets($fp); $ary = explode(",", $str); $server = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $username = $ary[0];
$str = fgets($fp); $ary = explode(",", $str); $password = $ary[0];
fclose($fp);

$sql = "UPDATE 'konrei' SET 'userno'=".$a[1].",'username'='".$a[1];
$sql = $sql."','kyosiki'='".$a[2]."','hirouen'='".$a[3];
$sql = $sql."','kaijyou'='".$a[4]."','mukotori'='".$a[5];
$sql = $sql."','sinrozoku'='".$a[6]."','sinroname1'='".$a[7]."','sinroname2'='".$a[8];
$sql = $sql."','sinpuzoku'='".$a[9]."','sinpuname1'='".$a[10]."','sinpuname2'='".$a[11];
$sql = $sql."','sinrodish'='".$a[12]."','sinpudish'='".$a[13];
$sql = $sql."','sinrosub'='".$a[13]."','sinpusub'='".$a[14]."' WHERE ('id'=".$recid.");";

if($mysql = mysql_pconnect($server,$username,$password)){
	mysql_select_db($dbname, $mysql);
	$ret = mysql_query($sql, $mysql);
	if($ret == FALSE){
		$retstr = "0,2,";
	}else{
		$retstr = "1,".$sql;
	}

	mysql_close($mysql);
}else{
	$retstr = "0,1,";
}
echo $retst;
?>
