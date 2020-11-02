<?php
$argvs = $_POST['com'];
//$argvs = "あいう$えお,かきくけこ,さしすせそ$#たちつてと,なにぬねの,はひふへ$ほ$$";
//mb_convert_variables('UTF-8', 'SJIS', $argvs);

// UTF8をShift_Jisに変換してメール送信
mb_internal_encoding("ISO-2022-JP");
$argvs = str_replace("$", "\r\n", $argvs);
$argvs = str_replace("!", "\r\n", $argvs);
$to = "obari-net@obari.co.jp";
//$to = "ops-sys@obari.co.jp";
//$to = "sasaki1957@gmail.com";
$from = "オバリ <mail@obari.co.jp>";
$rp = "mail@obari.co.jp";
$org = '株式会社オバリ';

$head = '';
$head .= "Content-Type: text/plain \r\n";
$head .= "Return-Path: $rp \r\n";
$head .= "From: $from \r\n";
$head .= "Sender: $from \r\n";
$head .= "Reply-To: $rp \r\n";
$head .= "Organization: $org \r\n";
$head .= "X-Sender: $from \r\n";
$head .= "X-Priority: 3 \r\n";

$subject = "Order Mail";
//$subject = mb_convert_encoding($subject, 'ISO-2022-JP-MS', 'UTF-8');
//$argvs = mb_convert_encoding($argvs, 'ISO-2022-JP-MS', 'UTF-8');
$argvs = base64_encode ($argvs);
//$subject = mb_convert_encoding($subject, 'SJIS', 'UTF-8');
//$argvs = mb_convert_encoding($argvs, 'SJIS', 'UTF-8');

if(mb_send_mail($to, $subject, $argvs, $head ,"-fmail@obari.co.jp" )){
	$ret = 'comp=1,,';
} else {
	$ret = 'comp=0,,';
}
echo $ret;
?>
