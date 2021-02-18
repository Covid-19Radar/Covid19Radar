# Covid19Radar/COCOA

このリポジトリでは接触感染アプリを大幅に改造しています。

## License
[MPLv2](LICENSE.txt)

## 行った主な変更：
* 大規模な結合 (競合の解決)
	* [Covid19Radar](https://github.com/Covid-19Radar/Covid19Radar) と [COCOA](https://github.com/cocoa-mhlw/cocoa) を結合。
	* プルリクエスト
		* [Update to my new github username #792](https://github.com/Covid-19Radar/Covid19Radar/pull/792)
		* [利用状況表示のUIが直感的でない #534 の実装 #705](https://github.com/Covid-19Radar/Covid19Radar/pull/705)
* 言語リソースを整理。
* ソースコードの大幅な整理。
	* null許容と非許容の区別。
* ユーザーデータに関する不具合を修正。
	* null検証を軽減した。
	* これに関連して、設定ページにチュートリアルを表示するボタンを追加。
* 最新情報ページに Wikipedia へのリンクを追加。
* ログを表示するUIを追加。

## これから行う変更：
* 最新情報ページに統計情報を表示。
* 言語リソースをもう少し整理。
