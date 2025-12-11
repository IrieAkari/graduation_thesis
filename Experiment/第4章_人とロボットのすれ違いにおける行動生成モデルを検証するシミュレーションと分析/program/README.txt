Editor:MATLAB
Version:R2022b


MATLABのActive inferenceシミュレーションの実行には，

1.AIFの解説である
Smith, R., Friston, K. J., & Whyte, C. J. (2022). 
A step-by-step tutorial on active inference and its 
application to empirical data. Journal of mathematical psychology, 
107, 102632.
より入手可能な
"Active-Inference-Tutorial-Scripts"

および

2.該当論文に導入方法が記述されている
"spm12"の全スクリプトの準備が推奨される．

resourceフォルダには，実行に最低限必要であると判断された（実行時に強制的に
ソースフォルダに選択させられたもの）コードを配置している．


また，spm12/toolbox/DEM/DEM.mを実行すると，AIFに関連する既存の研究の
ソースコードを一覧で確認可能であるので，参照すると良い．


また，Python形式で同様のAIFシミュレーション系が構築可能であり，詳細は
該当論文及びhttps://github.com/infer-actively/pymdpで確認可能である．

元プログラムは
spm12/toolbox/DEM/DEM.mを実行後，Maze learningを参照
