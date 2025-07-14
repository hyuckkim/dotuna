using System.Windows.Forms;

namespace DoTuna
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
            AddTooltip(
                AddSettingControl("패턴", Setting.Instance.Pattern, v => Setting.Instance.Pattern = v),
                "각 문서의 제목입니다.\n" +
                "{id}, {title}, {name}, {created}, {updated}, {size}가\n" +
                "실제 값으로 대체됩니다.\n\n" +
                "글자 자르기:\n" +
                "{title 10..}  → 앞 10글자만 사용하고 잘리면 '..' 추가\n" +
                "{name _20}    → 뒤 20글자만 사용하고 잘리면 '_' 추가\n" +
                "{user 10_10}  → 앞 10글자, 뒤 10글자 사용\n\n" +
                "예: \"{title} - {name} ({created})\""
            );
            AddTooltip(
                AddSettingControl("이미지 임베딩", Setting.Instance.SingleHTML, v => Setting.Instance.SingleHTML = v),
                "이미지를 html 문서 안에 포함합니다.\n" +
                "이미지 파일이 따로 생성되지 않아, html 파일만 보관할 수 있습니다.\n" +
                "이미지가 base64로 인코딩되어 이미지의 용량이 33%정도 커집니다."
            );
            AddTooltip(
                AddSettingControl("css 파일 사용", Setting.Instance.UseCssFile, v => Setting.Instance.UseCssFile = v),
                "css 파일을 사용합니다.\n" +
                "css 파일이 따로 생성되어 모든 스레드 파일들이 css 파일을 공유합니다.\n" +
                "이로 인해 용량이 아주 조금 줄어듭니다.\n" +
                "css 파일을 사용하지 않으면, html 문서 안에 css가 포함됩니다."
            );
            AddTooltip(
                AddSettingControl("인덱스 페이지 미리 생성", Setting.Instance.PreloadIndex, v => Setting.Instance.PreloadIndex = v),
                "인덱스 html 페이지를 미리 생성합니다.\n" +
                "생성하지 않으면 인덱스 페이지를 열 때마다 로컬 함수가 동작해 페이지를 생성합니다.\n" +
                "생성하면 인덱스 페이지의 용량이 아주 조금 커집니다."
            );
        }
    }
}
