// stdafx.h : �W���̃V�X�e�� �C���N���[�h �t�@�C���̃C���N���[�h �t�@�C���A�܂���
// �Q�Ɖ񐔂������A�����܂�ύX����Ȃ��A�v���W�F�N�g��p�̃C���N���[�h �t�@�C��
// ���L�q���܂��B
//

#pragma once
#pragma comment(lib, "Ws2_32.lib")

#include "targetver.h"

#include <stdio.h>
#include <tchar.h>
#include <ws2tcpip.h>
#include <iostream>
#include <string>
#include <winsock2.h>

// TODO: �v���O�����ɕK�v�Ȓǉ��w�b�_�[�������ŎQ�Ƃ��Ă��������B
class MySender
{
private:
	struct sockaddr_in addr;
	WSAData wsaData;
	SOCKET sock;
public:
	MySender() {
	}

	~MySender()//disconnect�Y��h�~
	{
		closesocket(sock);
		WSACleanup();
	}

	void MySender::connect(char* address, int port)
	{
		WSAStartup(MAKEWORD(2, 0), &wsaData);
		addr.sin_family = AF_INET;
		addr.sin_port = htons(port);
		inet_pton(AF_INET, address, &addr.sin_addr.S_un.S_addr);
		sock = socket(AF_INET, SOCK_DGRAM, 0);
	}

	void MySender::disconnect()
	{
		closesocket(sock);
		WSACleanup();
	}
	void MySender::send(char* message) {
		sendto(sock, message, strlen(message), 0, (struct sockaddr *)&addr, sizeof(addr));
	}
};



class MyReceiver
{
private:
	WSAData wsaData;
	SOCKET sock;
	struct sockaddr_in addr;
	char buf[2048];			//2048�����܂�
public:

	void connect(int port)
	{
		WSAStartup(MAKEWORD(2, 0), &wsaData);

		sock = socket(AF_INET, SOCK_DGRAM, 0);

		addr.sin_family = AF_INET;
		addr.sin_port = htons(port);
		addr.sin_addr.S_un.S_addr = INADDR_ANY;

		bind(sock, (struct sockaddr *)&addr, sizeof(addr));

		memset(buf, 0, sizeof(buf));//buf�̏�����
	}
	void disconnect()
	{
		closesocket(sock);
		WSACleanup();
	}
	char* recvwait() {
		recv(sock, buf, sizeof(buf), 0);//��M�҂�
		return buf;
	}

};