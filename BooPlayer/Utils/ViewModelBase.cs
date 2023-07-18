using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BooPlayer.Utils; 
/**
 * IDisposable なプロパティをDisposeするかどうかを指定するためのアノテーションクラス
 * 
 * 使用例は、ViewModelBase#Dispose()を参照
 */
public class Disposal : Attribute {
    public bool ToBeDisposed { get; }
    public Disposal(bool disposable = true) {
        ToBeDisposed = disposable;
    }
}

/**
 * INotifyPropertyChanged i/f を実装したViewModelの基底クラス
 */
public class PropertyChangeNotifier : INotifyPropertyChanged {
    #region INotifyPropertyChanged i/f
    //-----------------------------------------------------------------------------------------

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void notify(string propName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
    protected string callerName([CallerMemberName] string memberName = "") {
        return memberName;
    }

    protected bool setProp<T>(string name, ref T field, T value, params string[] familyProperties) {
        if (field != null ? !field.Equals(value) : value != null) {
            field = value;
            notify(name);
            foreach (var p in familyProperties) {
                notify(p);
            }
            return true;
        }
        return false;
    }

    #endregion
}

/**
 * PropertyChangeNotifier に、Disposableなプロパティの自動的な解放機能を追加したクラス。
 */
public class ViewModelBase : PropertyChangeNotifier, IDisposable {
    /**
     * コンストラクタ
     * @param disposeNonPublic true: Non-Public なプロパティも Disposeする
     */
    public ViewModelBase(bool disposeNonPublic = false) {
        DisposeNonPublic = disposeNonPublic;
    }

    // Non-Public なプロパティも Disposeするか？ (通常はコンストラクタで指定する）
    public bool DisposeNonPublic { get; set; } = false;

    /**
     * 列挙されたプロパティのDisposeを呼びまわる
     */
    private void DisposeProps(PropertyInfo[] props) {
        // if(null==props) {
        //     return;
        // }
        foreach (var prop in props) {
            // インデクサを除外する
            //  インデクサに対して GetValue()すると例外が出る。
            //  インデクサは表記をちょっとだけ簡潔にする「シンタックスシュガー的」に定義すると思うので、普通は他の方法でDisposeするはず。
            var indexer = prop.GetIndexParameters();
            if (indexer.Length == 0) {
                // var obj = prop.GetValue(this);
                if (prop.GetValue(this) is IDisposable obj) {
                    var attrs = prop.GetCustomAttributes(false).Where((v) => v is Disposal);
                    if ((attrs.FirstOrDefault() as Disposal)?.ToBeDisposed ?? true) {
                        obj.Dispose();
                    }
                }
            }
        }
    }

    /**
     * Disposable な プロパティをすべてDisposeする。
     * ここでDisposeしては困るプロパティには、[Disposal(false)] を指定すること。
     */
    public virtual void Dispose() {
        var type = this.GetType();

        // Public なプロパティのDispose
        DisposeProps(type.GetProperties());

        // DisposeNonPublic == true なら、private/protectedなプロパティもDisposeする
        if (DisposeNonPublic) {
            DisposeProps(type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));
        }
    }
}

/**
 * PropertyChangeNotifier に、Disposableなプロパティの自動的な解放機能と、
 * WeakReferenceなOwnerを保持するための仕掛けを追加した究極のViewModelベースクラス。
 */
public class ViewModelBase<T> : ViewModelBase where T : class {
    private WeakReference<T>? mOwner;
    [Disposal(false)]
    public T? Owner {
        get => mOwner.GetValue();
        set => mOwner = (value == null) ? null : new WeakReference<T>(value);
    }

    public ViewModelBase(T? owner=null, bool disposeNonPublic=false )
        : base(disposeNonPublic) {
        Owner = owner;
    }
}