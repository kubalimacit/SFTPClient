﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Bu kod araç tarafından oluşturuldu.
//     Çalışma Zamanı Sürümü:4.0.30319.42000
//
//     Bu dosyada yapılacak değişiklikler yanlış davranışa neden olabilir ve
//     kod yeniden oluşturulursa kaybolur.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SFTPClient {
    using System;
    
    
    /// <summary>
    ///   Yerelleştirilmiş dizeleri aramak gibi işlemler için, türü kesin olarak belirtilmiş kaynak sınıfı.
    /// </summary>
    // Bu sınıf ResGen veya Visual Studio gibi bir araç kullanılarak StronglyTypedResourceBuilder
    // sınıfı tarafından otomatik olarak oluşturuldu.
    // Üye eklemek veya kaldırmak için .ResX dosyanızı düzenleyin ve sonra da ResGen
    // komutunu /str seçeneğiyle yeniden çalıştırın veya VS projenizi yeniden oluşturun.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Bu sınıf tarafından kullanılan, önbelleğe alınmış ResourceManager örneğini döndürür.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SFTPClient.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Tümü için geçerli iş parçacığının CurrentUICulture özelliğini geçersiz kular
        ///   CurrentUICulture özelliğini tüm kaynak aramaları için geçersiz kılar.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///    Connection failed  {0} benzeri yerelleştirilmiş bir dize arar.
        /// </summary>
        internal static string ConnectionFailedMessage {
            get {
                return ResourceManager.GetString("ConnectionFailedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Connected to {0} benzeri yerelleştirilmiş bir dize arar.
        /// </summary>
        internal static string ConnectionStatusConnected {
            get {
                return ResourceManager.GetString("ConnectionStatusConnected", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Not Connected benzeri yerelleştirilmiş bir dize arar.
        /// </summary>
        internal static string ConnectionStatusDisconnected {
            get {
                return ResourceManager.GetString("ConnectionStatusDisconnected", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Failed to connect to {0} benzeri yerelleştirilmiş bir dize arar.
        /// </summary>
        internal static string ConnectionStatusFailed {
            get {
                return ResourceManager.GetString("ConnectionStatusFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Connected successfully! benzeri yerelleştirilmiş bir dize arar.
        /// </summary>
        internal static string ConnectionSuccessMessage {
            get {
                return ResourceManager.GetString("ConnectionSuccessMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Delete benzeri yerelleştirilmiş bir dize arar.
        /// </summary>
        internal static string DeleteButtonText {
            get {
                return ResourceManager.GetString("DeleteButtonText", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Connection disconnected by the user. benzeri yerelleştirilmiş bir dize arar.
        /// </summary>
        internal static string DisconnectionMessage {
            get {
                return ResourceManager.GetString("DisconnectionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Save benzeri yerelleştirilmiş bir dize arar.
        /// </summary>
        internal static string SaveButtonText {
            get {
                return ResourceManager.GetString("SaveButtonText", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Search... benzeri yerelleştirilmiş bir dize arar.
        /// </summary>
        internal static string SearchPlaceholder {
            get {
                return ResourceManager.GetString("SearchPlaceholder", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Settings benzeri yerelleştirilmiş bir dize arar.
        /// </summary>
        internal static string SettingsTitle {
            get {
                return ResourceManager.GetString("SettingsTitle", resourceCulture);
            }
        }
    }
}
