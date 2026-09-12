#!/bin/bash
set -e

echo "=============================="
echo "  MCR - Setup inicial VPS"
echo "=============================="

IP_VPS="$1"
SSH_USER="${2:-root}"
DEPLOY_DIR="${3:-mcr}"

if [ -z "$IP_VPS" ]; then
    echo "Uso: ./setup-vps.sh <IP_VPS> [user] [deploy_dir]"
    echo "   ./setup-vps.sh 13.140.35.214"
    echo "   ./setup-vps.sh 217.216.89.186 root mcr"
    exit 1
fi

echo ""
echo "1/4 - Instalando Docker..."
ssh "$SSH_USER@$IP_VPS" "bash -s" << 'REMOTE'
    if ! command -v docker &>/dev/null; then
        curl -fsSL https://get.docker.com | sh
        apt install -y docker-compose-plugin
        echo "Docker instalado."
    else
        echo "Docker ja instalado."
    fi
REMOTE

echo ""
echo "2/4 - Instalando Git..."
ssh "$SSH_USER@$IP_VPS" "apt-get install -y git || echo 'Git ja instalado'"

echo ""
echo "3/4 - Clonando repo e configurando..."
ssh "$SSH_USER@$IP_VPS" "bash -s" << REMOTE
    mkdir -p /opt/$DEPLOY_DIR
    cd /opt/$DEPLOY_DIR
    if [ ! -d .git ]; then
        git clone https://github.com/txnenterprise/mcr.git .
    else
        git pull origin main
    fi
    mkdir -p data/dataprotection-keys
REMOTE

echo ""
echo "4/4 - Setup concluido!"
echo ""
echo "Proximo passo:"
echo "  1. Configure as GitHub Secrets:"
echo "     - VPS_MCR_HOST / VPS_MCR_SSH_KEY (para 217.216.89.186)"
echo "     - VPS_COCAMAR_HOST / VPS_COCAMAR_SSH_KEY (para 13.140.35.214)"
echo "  2. Configure o DNS para apontar para a VPS"
echo "  3. Push para main ativa o deploy automatico"
