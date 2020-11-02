<?php
$dbname = $_POST['com'];
$sqlstr = $_POST['sql'];
$filds = $_POST['fld'];

$retstr = "dbname<".$dbname."> sql<".$sqlstr."> filds<".$filds.">";
echo $retstr;
?> 
