﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OnCallSchedulerTests.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("OnCallSchedulerTests.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
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
        ///   Looks up a localized string similar to [{&quot;Name&quot;:&quot;Dr. Acula&quot;,&quot;CantWorkOn&quot;:[&quot;2014-08-20&quot;,&quot;2014-08-21&quot;],&quot;PointReduction&quot;:2},
        ///{&quot;Name&quot;:&quot;Dr. Who&quot;,&quot;CantWorkOn&quot;:[&quot;2014-08-01&quot;,&quot;2014-08-02&quot;,&quot;2014-08-03&quot;]},
        ///{&quot;Name&quot;:&quot;NotARealDoctor&quot;,&quot;CantWorkOn&quot;:[&quot;2014-08-09&quot;,&quot;2014-08-10&quot;]},
        ///{&quot;Name&quot;:&quot;Pickles&quot;,&quot;CantWorkOn&quot;:[&quot;2014-08-12&quot;,&quot;2014-08-14&quot;]},
        ///{&quot;Name&quot;:&quot;Dr. Foo&quot;,&quot;CantWorkOn&quot;:[&quot;2014-07-31&quot;]},
        ///{&quot;Name&quot;:&quot;Dr. Bar&quot;}].
        /// </summary>
        internal static string AgentsJson {
            get {
                return ResourceManager.GetString("AgentsJson", resourceCulture);
            }
        }
    }
}
