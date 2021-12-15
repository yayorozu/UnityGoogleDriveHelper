# UnityGoogleDriveHelper

UnityWebRequest を利用して Google Drive を利用できるライブラリ

* SpreadSheet の読み書き
* ディレクトリ一覧の表示
* 画像のロードに対応しています

公開ファイルじゃない場合は Google Developers Console より ClientId と ClientSecret を発行する必要があります

## Sample Window

サンプルとして、3つのEditorWindowがある

#### Tools/GoogleDriveSample/Folder

ディレクトリ一覧を表示する


#### Tools/GoogleDriveSample/ShareSpreadSheet
公開されている Spread Sheet の読み込み


#### Tools/GoogleDriveSample/SpreadSheet
Spread Sheet の読み込み

## Sample Code

### 公開SpreadSheetへのアクセス

```cs
GoogleSpreadSheetApi.GetSheet(spreadSheetId, sheetName, v => /* string[,] */);
```


### 公開SpreadSheet のデータをクラスへキャストへのアクセス

```cs
private class Sample
{
    public int IntValue;
}

GoogleSpreadSheetApi.GetSheet<Sample>(spreadSheetId, sheetName, v => /* List<Sample> */);
```

### OAuth を使ったアクセス

```cs
// ClientId と ClientSecret を登録
var clientData = GoogleOAuthClientData.Load("GoogleAuthData");
var data = new GoogleSpreadSheetOAuthData(clientData);
data.SetToken(new EditorOAuthToken(EditorApplication.applicationPath));

// ブラウザを開いて認証コードを取得
data.GetOAuthCode();

// 取得した認証コードを適応してアクセストークンを取得
data.SetOAuthCode(code);
data.GetToken();

// Spread Sheet のロード
data.LoadSpreadSheet(_spreadSheetId, data =>
{
    // SpreadSheet の情報を取得
});

// SpreadSheet のデータをもとにシートをロード
data.LoadSheet(_sheetData, sheet, v =>
{
    // string[,]
});
```

