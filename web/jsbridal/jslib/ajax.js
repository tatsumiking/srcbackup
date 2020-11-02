// XMLHttpRequest�I�u�W�F�N�g����
function createHttpRequest()
{
  var xmlhttp = null;
  if(window.ActiveXObject){
    try {
      // MSXML2�ȍ~�p
      xmlhttp = new ActiveXObject("Msxml2.XMLHTTP");
    } catch (e) {
      try {
        // ��MSXML�p
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
      } catch (e2) {

      }
    }
  } else if(window.XMLHttpRequest){
    // Win Mac Linux m1,f1,o8 Mac s1 Linux k3�p
    xmlhttp = new XMLHttpRequest();
  } else {

  }
  if (xmlhttp == null) {
    alert("Can not create an XMLHTTPRequest instance");
  }
  return xmlhttp;
} 

// �t�@�C���ɃA�N�Z�X����M���e���m�F���܂�
function sendRequest (method, url, data, async, callback)
{
    // XMLHttpRequest�I�u�W�F�N�g����
    var xmlhttp = createHttpRequest();
    
    // ��M���ɋN������C�x���g
    xmlhttp.onreadystatechange = function() { 
        // readyState�l��4�Ŏ�M����
        if (xmlhttp.readyState == 4) { 
            //�R�[���o�b�N
            callback(xmlhttp);
        }
    }
    // open ���\�b�h
    xmlhttp.open(method, url, async);
    // HTTP���N�G�X�g�w�b�_��ݒ�
    xmlhttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    // send ���\�b�h
    xmlhttp.send(data);
}
