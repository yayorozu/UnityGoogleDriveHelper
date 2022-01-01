# UnityGoogleDriveHelper

UnityWebRequest を利用して Google Drive を利用できるライブラリ

現状以下の機能に対応しています

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

### AppsScript を利用する

ランタイムでは Private な Google Drive にアクセスするのは厳しいので
AppsScript を利用してお手軽にアクセスできる

```cs
var deployId = "";

var json = "";
// 独自Json を利用して AppsScript を POST で呼び出す
GoogleSpreadSheetApi.PostAppsScript(deployId, json);

var sheetName = "シート1";
var rows = new[] {"a", "b"};

// 以下のスクリプトを利用して指定したシートの最後に文字を追加する
GoogleSpreadSheetApi.PostAppsScript(deployId, sheetName, rows, 1);
```

```
function doPost(e) {
  var params = JSON.parse(e.postData.getDataAsString());

  var text = addLast(params);

  // レスポンス作成
  var output = ContentService.createTextOutput();
  output.setMimeType(ContentService.MimeType.JSON);
  output.setContent(JSON.stringify({ message: text }));
  return output;
}

// 最後に追加する
function addLast(params) {
  var sheetName = params.sheetName;
  var rows = params.rows;
  var skip = params.skip;

  if (sheetName == null || rows == null)
    return "Invalid Param";
  
  if (skip == null)
    skip = 0;

  var active = SpreadsheetApp.getActive();
  var sheet = active.getSheetByName(sheetName);
  if (sheet == null)
    return "Sheet Not Found";
  
  var last = sheet.getLastRow();
  // rowsは2次元配列
  var count = rows[0].length;
  if (count <= 0)
    return "Rows is Empty";
  
  var cell = sheet.getRange(last + 1, 1 + skip, 1, count);
  cell.setValues(rows);

  // 成功した場合は空文字列を返す
  return "";
}
```