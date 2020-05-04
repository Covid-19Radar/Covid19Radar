# iOS版アプリのテスターの方へ

「COVID-19 Radar」アプリのテストにご参加いただきありがとうございます。  
本ドキュメントでは、テスターに向けてテストリリースのインストール方法をご紹介します。

## 前提条件
以下の手続きが完了している事を前提とします。
- [ベータテスターへの応募](https://bit.ly/2XSuVUJ) 
- ベータテスターへ応募した際のメールアドレスにて、  
Microsoftアカウント、もしくはGitHubアカウントの作成
- ブラウザは標準のSafariを使用
- Safariは「プライベートモード」ではないこと

## ドキュメント作成環境
本ドキュメントでは、以下の環境にてスクリーンショットの取得・動作確認を行っております。  
テスターの皆様の環境に応じて、適宜読み替えをお願いします。
- Apple iPhone SE
- Apple iPhone 7
- iOS 13.3.1

---
## デバイスの追加方法
新規デバイスにテストリリースをインストールする場合、デバイスの登録が必要です。  
以下の手順に従い、デバイスの登録をお願いします。

### 1. App Center にサインイン

[App Center](https://appcenter.ms/sign-in) に、iOS端末 でアクセスします。  
標準カメラアプリより、以下のQRコードをスキャンしてもアクセス可能です。  
![App Center QRCode](../.attachments/appcenter-qrcode.png)

AppCenterのサインインページより、テスター登録時にFormに入力したメールアドレスでサインインをしてください。  
![App Centerにサインイン](../.attachments/iOS_001_appcenter_signin.png)

### 2. プロファイルのインストール
新しいデバイスへのインストールを行う際は、AppCenterへのデバイス登録、およびプロファイルのインストールを行う必要があります。

My Appsの画面にAndroid版とiOS版のアプリが表示されています。
「COVID-19 Radar」のiOSアプリをタップします。  
![App Centerでアプリをダウンロード](../.attachments/iOS_002_appcenter_selectapps.png.png)


ダウンロードページへのQRコードが表示されますので、下部のリンク（赤枠部）をタップします。  
![App Centerでアプリをダウンロード](../.attachments/iOS_003_appcenter_installpage-qr.png)

デバイスの追加についての案内が表示されますので、新規のデバイスの場合は「+Add Device」をタップします。  
なお、すでに登録が完了しているデバイスの場合は、ボタン下部の「I'm good」をタップしてください。  
![App Centerでデバイスを追加](../.attachments/iOS_004_appcenter_add-devices.png)

構成プロファイルのダウンロードが始まりますので、「許可」をタップします。  
![App Centerでデバイスを追加](../.attachments/iOS_005_appcenter_installprofile.png)

プロファイルがダウンロードされたら、下記の表示になりますので、「閉じる」をタップします。  
![App Centerでデバイスを追加](../.attachments/iOS_006_appcenter_installprofile.png)

「設定」アプリを開くと、「プロファイルがダウンロードされました」というメッセージが表示されていますので、タップして開きます。  
![プロファイルのインストール](../.attachments/iOS_007_ConfigApp_installprofile.png)

プロファイルの詳細が表示されますので、右上の「インストール」をタップします。  
![プロファイルのインストール](../.attachments/iOS_008_ConfigApp_installprofile.png)

デバイスに設定されたパスコード・パスワードを入力します。  
![プロファイルのインストール](../.attachments/iOS_009_ConfigApp_installprofile.png)

インストールの確認画面が表示されますので、「インストール」をタップします。  
![プロファイルのインストール](../.attachments/iOS_010_ConfigApp_installprofile.png)

プロファイルのインストールが成功すると、SafariでAppCenterのページが表示されます。  
ページ下部の「Show previous versions」をタップします。
![プロファイルのインストール](../.attachments/iOS_011_appcenter_installedprofile_page.png)


テストリリースの一覧が表示されますが、AppCenterでデバイスの登録が完了するまでの間、「INSTALL」ボタンは表示されません。  
![プロファイルのインストール](../.attachments/iOS_012_appcenter_installpage_notinstall.png)


デバイスの登録が完了するまで、最長で4時間程度掛かる場合があります。  
お茶でも飲んでお待ちください。

## アプリのインストール
デバイスの登録が完了しましたら、以下の手順に従ってアプリのインストールをお願いします。

### 1. App Center にサインイン

[インストール画面](https://install.appcenter.ms/orgs/Covid19Radar/apps/Covid19RadarIOS) に、iOS端末 でアクセスします。  
標準カメラアプリより、以下のQRコードをスキャンしてもアクセス可能です。  
![Covid19RadarIOS QR Code](../.attachments/appcenter-install-ios.png)  
必要に応じて、AppCenterへのサインインを行います。

デバイスの登録についての画面が表示される場合がありますので、「I'm good.」をタップします。  
![インストール画面](../.attachments/iOS_013_add-device-good.png)

テストリリースのインストールについて確認が表示されますので、「OK」をタップします。  
![インストール画面](../.attachments/iOS_014_add-device-good.png)

現在リリースされているバージョンの一覧が表示されていますので、最上部の「Latest release」の「INSTALL」ボタンをタップします。  
![ブラウザからのインストール許可](../.attachments/iOS_015_install-apps.png)

アプリのインストールについて確認が表示されますので、「インストール」をタップします。  
![App Centerでアプリをダウンロード](../.attachments/iOS_016_install-apps.png)

バックグラウンドでアプリがインストールされ、ホーム画面に追加されます。  
![App Centerでアプリをダウンロード](../.attachments/iOS_017_install-apps.png)  
![App Centerでアプリをダウンロード](../.attachments/iOS_018_install-apps.png)


以上で、アプリのインストールは完了です。

---
## アプリのアップデート方法
アプリのテストビルドが終わる都度、最新版の通知が登録されたメールアドレスに届きますので、以下の手順で最新のアプリのダウンロード・インストールを行います。

### 1. メールの確認
テスト版のリリースについてはメールにて通知されておりますので、最新のアプリを手動でインストールします。  
![メールでの通知](../.attachments/iOS_019_update-notify-mail.png)

最新のリリースについて、以下のようなメールが届きますので、「Install」をタップしてAppCenterのダウンロードページを開きます。  
![メールでの通知](../.attachments/iOS_020_update-notify-mail.png)

## 2. アプリのダウンロード
該当リリースのダウンロードページが開きますので、「INSTALL」をタップして、アプリをダウンロードします。  
![AppCenter テストビルドのダウンロード](../.attachments/iOS_021_install-apps.png)

アプリのインストールについて確認が表示されますので、「インストール」をタップします。  
![App Centerでアプリをダウンロード](../.attachments/iOS_022_install-apps.png)  
バックグラウンドでアプリが更新されます。

以上で、アプリのアップデートは完了です。

---
## アプリの利用方法
以下のドキュメントを参照してください。  
[Covid19Radar(GitHub) - HOW_TO_USE for iOS](https://github.com/Covid-19Radar/Covid19Radar/blob/master/doc/How-to-use/iPhone-how-to-use.ja.md)

---
## バグを発見した場合
バグの報告、および新機能の追加の提案をされたい場合、GitHub上リポジトリにて、Issueの作成をお願いします。  
詳細は、リポジトリ内の「HOW_TO_CONTRIBUTE」をご覧ください。  
[Covid19Radar(GitHub) - HOW_TO_CONTRIBUTE.md](https://github.com/Covid-19Radar/Covid19Radar/blob/master/HOW_TO_CONTRIBUTE.md)

## 相談をしたい場合
チャットアプリケーション([Discord](https://discordapp.com/))にて、コントリビューター同士のコミュニケーションを行っております。  
下記招待リンクより参加をお願いします。  
[Covid19Radar(Discord)](https://discord.gg/EzaYhD)

-----
「COVID-19 Radar」は現在開発中のアプリです。  
コミットする都度、最新版の通知がメールアドレスに来ますので、最新版を都度ダウンロードいただきますよう、よろしくお願いいたします。