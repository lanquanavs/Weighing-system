using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AWSV2.Services
{
    public class ListBoxScrollToBottomBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            //只有ICollectionView类型才有CollectionChanged事件
            //所以这里强转了一下类型
            ((ICollectionView)AssociatedObject.Items).CollectionChanged += ListBoxScrollToBottomBehavior_CollectionChanged;
        }
        private void ListBoxScrollToBottomBehavior_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //这里正好使用了ListBox的ScrollIntoView()方法，也只有ListBox类型才有这个方法
            //其父类没有这里方法，所以T使用的ListBox,没有使用其父类
            if (AssociatedObject.HasItems)
                AssociatedObject.ScrollIntoView(AssociatedObject.Items[AssociatedObject.Items.Count - 1]);
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            ((ICollectionView)AssociatedObject.Items).CollectionChanged -= ListBoxScrollToBottomBehavior_CollectionChanged;
        }
    }

}
