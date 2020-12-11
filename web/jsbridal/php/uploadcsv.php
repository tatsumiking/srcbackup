<?php

$pccsvfilename = $_POST['file'];
$base64data = $_POST['data'];
$base64data = str_replace(' ' , '+' , $base64data);
$ary = split(',',$base64data);
$textdata = base64_decode($ary[1]);
$ret = file_put_contents($pccsvfilename, $textdata);
if($ret == 0){
	$str = "0,".$pccsvfilename.",";
}else{
	$str = "1,".$pccsvfilename.",";
}
echo $str;

?>

