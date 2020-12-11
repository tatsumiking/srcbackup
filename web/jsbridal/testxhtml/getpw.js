	m_aryIdx = [
		[0,1,3,7,8],
		[1,2,6,8,9],
		[2,4,5,7,8],
		[1,3,4,6,9],
		[0,2,3,7,8],
		[0,1,4,5,6],
		[2,3,5,6,7],
		[0,1,2,5,6],
		[0,3,6,7,9],
		[1,2,4,5,9]
	];
	m_aryPW = ["9","5","7","2","6","4","3","8","0","1"];
	m_csKey = "egassem";
	klen = m_csKey.length;
	strPW = "";
	max = strID.length;
	pid = 0;
	for(idx = 0; idx < max; idx++){
		num = 0;
		for(aidx = 0; aidx < 5; aidx++){
			tidx = m_aryIdx[idx][aidx];
			if(tidx < max){
				ch = strID.substr(tidx, 1);
				num = num + ch.charCodeAt(0);
			}
		}
		kidx = idx % klen;
		ch = m_csKey.substr(kidx, 1);
		num = num + ch.charCodeAt(0);
		tidx= (num + idx) % 10;
		strPW = strPW+m_aryPW[tidx];
	}

