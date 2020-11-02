<?php

$str = "123";
$num = intval($str);
$str = "<".$num.">\r\n";
echo $str;
$str = "abc";
$num = intval($str);
$str = "<".$num.">\r\n";
echo $str;

?>
