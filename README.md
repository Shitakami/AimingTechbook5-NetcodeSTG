# はじめに

このリポジトリは『Aiming Tech Book Vol.5：Netcode for GameObjectsで作る遅延を感じにくいゲーム設計』のサンプルプロジェクトです。

# 環境

* Unity 6000.0.41f
* Netcode for GameObjects 2.3.1
* Multiplayer Play Mode 1.3.3
* Multiplayer Tools 2.2.3

サンプルゲームで使用しているライブラリ

* R3 1.3.0
* UniTask 2.5.10
* ObservableCollections 3.3.3

# シーンの実行方法

シーンを再生するまえに、Widnow > Multiplayer > Multiplayer Play Mode を起動し、Virtual Playersを起動することを推奨します。

<img src="https://github.com/user-attachments/assets/2fbf99f7-5830-4d7c-b99a-bc419340bfa3" width="70%">

また、遅延を設定したい場合は、シーン上のNetworkSimulatorから `Connection Preset` を変更して設定できます。

<img src="https://github.com/user-attachments/assets/4630f0df-f782-4e7d-b7f1-870f882c7c84" width="70%">


## Netcode Sample

1. シーンを再生
2. 最初にいずれかでHostボタンをクリック、ホストで参加
3. ほかのPlayerでClientボタンをクリック、Clientで参加
4. 挙動を確認
   * 接続時に生成されるPlayerオブジェクトを矢印キーで操作
   * BroadcastRpc/ServerRpcボタンをクリックして、ログで通信を確認

## Shooting Game

1. シーンを再生
2. 最初にいずれかでHostボタンをクリック、ホストで参加
3. ほかのPlayerでClientボタンをクリック、Clientで参加
4. ホストとすべてのクライアントでReadyボタンをクリック
5. 全員がReady状態にして、敵を生成
6. プレイヤーを矢印キーで移動、スペースキーで弾を発射
7. 敵の体力を0にすると終了

※ Shooting Gameシーンは自動的に接続、Ready状態になるように設定しています。設定を切りたい場合は、`AutoConnectionAndReadySetting` の `Auto Connect` `Auto Ready` からチェックを外してください。

