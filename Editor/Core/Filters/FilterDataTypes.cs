
namespace ME.ECSEditor {

    using ME.ECS;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    
    [UnityEditor.CustomPropertyDrawer(typeof(ME.ECS.FilterDataTypes))]
    public class FilterDataTypesEditor : UnityEditor.PropertyDrawer {

        private const float headerHeight = 22f;
        private const float miniHeight = 16f;
        private const float lineHeight = 26f;
        private const float editButtonHeight = 40f;
        private const float marginBottom = 10f;

        private Rect DrawArray(UnityEngine.Rect position, SerializedProperty property, string name, ComponentDataTypeAttribute.Type drawType) {
            
            var list = new System.Collections.Generic.List<System.Type>();
            var usedComponents = new System.Collections.Generic.HashSet<System.Type>();
            {
                //var backStyle = new GUIStyle(EditorStyles.label);
                //backStyle.normal.background = Texture2D.whiteTexture;
                
                var with = property.FindPropertyRelative(name);
                var size = with.arraySize;
                for (int i = 0; i < size; ++i) {

                    var registry = with.GetArrayElementAtIndex(i);
                    FilterDataTypesEditor.GetTypeFromManagedReferenceFullTypeName(registry.managedReferenceFullTypename, out var type);

                    if (type == null) {

                        Debug.Log("Not found: " + registry.managedReferenceFullTypename + ", " + registry.managedReferenceFieldTypename);
                        continue;

                    }
                    
                    list.Add(type);
                    usedComponents.Add(type);

                    position.height = FilterDataTypesEditor.lineHeight;

                    var backRect = EditorGUI.IndentedRect(position);
                    backRect.x -= 8f;
                    backRect.width += 8f;
                    if (drawType == ComponentDataTypeAttribute.Type.WithData) {

                        var copy = registry.Copy();
                        var initDepth = copy.depth;
                        if (copy.NextVisible(copy.hasChildren) == true) {
                            ++EditorGUI.indentLevel;
                            do {

                                if (copy.depth <= initDepth) break;
                                var h = EditorGUI.GetPropertyHeight(copy, true);
                                backRect.height += h;

                            } while (copy.NextVisible(false) == true);
                            --EditorGUI.indentLevel;
                            backRect.height += 4f;
                        }

                    } else if (drawType == ComponentDataTypeAttribute.Type.NoData) { }
                    EditorGUI.DrawRect(backRect, new Color(0f, 0f, 0f, i % 2 == 0 ? 0.2f : 0.15f));
                    
                    {
                        var componentName = GUILayoutExt.GetStringCamelCaseSpace(type.Name);
                        EditorGUI.LabelField(position, componentName, EditorStyles.boldLabel);
                    }

                    position.y += lineHeight;

                    if (drawType == ComponentDataTypeAttribute.Type.WithData) {

                        var copy = registry.Copy();
                        var initDepth = copy.depth;
                        if (copy.NextVisible(copy.hasChildren) == true) {
                            ++EditorGUI.indentLevel;
                            do {

                                if (copy.depth <= initDepth) break;
                                var h = EditorGUI.GetPropertyHeight(copy, true);
                                position.height = h;
                                EditorGUI.PropertyField(position, copy, true);
                                position.y += h;

                            } while (copy.NextVisible(false) == true);
                            --EditorGUI.indentLevel;
                        }

                    } else if (drawType == ComponentDataTypeAttribute.Type.NoData) { }

                }
            }
            {
                var obj = property.serializedObject;
                position.height = editButtonHeight;
                GUILayoutExt.DrawAddComponentMenu(position, usedComponents, (addType, isUsed) => {
                    
                    obj.Update();
                    var prop = obj.FindProperty(property.propertyPath);
                    var with = prop.FindPropertyRelative(name);
                    if (isUsed == true) {

                        usedComponents.Remove(addType);
                        with.DeleteArrayElementAtIndex(list.IndexOf(addType));
                        list.Remove(addType);

                    } else {

                        usedComponents.Add(addType);
                        list.Add(addType);
                        ++with.arraySize;
                        var item = with.GetArrayElementAtIndex(with.arraySize - 1);
                        item.managedReferenceValue = (IComponentBase)System.Activator.CreateInstance(addType);

                    }
                    obj.ApplyModifiedProperties();

                }, showRuntime: true);
            }

            position.y += FilterDataTypesEditor.editButtonHeight;
            return position;

        }
        
        private ComponentDataTypeAttribute GetAttr() {

            var attrs = this.fieldInfo.GetCustomAttributes(typeof(ComponentDataTypeAttribute), true);
            return (attrs.Length == 1 ? (ComponentDataTypeAttribute)attrs[0] : null);

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

            var drawType = ComponentDataTypeAttribute.Type.NoData;
            if (this.GetAttr() is ComponentDataTypeAttribute attr) {

                drawType = attr.type;

            }
            
            var h = 0f;
            h += FilterDataTypesEditor.headerHeight;
            
            h += FilterDataTypesEditor.miniHeight;
            h += this.GetArrayHeight(property, "with", drawType);
            h += editButtonHeight;
            
            h += FilterDataTypesEditor.miniHeight;
            h += this.GetArrayHeight(property, "without", ComponentDataTypeAttribute.Type.NoData);
            h += editButtonHeight;
            
            return h + marginBottom;
            
        }

        private float GetArrayHeight(SerializedProperty property, string name, ComponentDataTypeAttribute.Type drawType) {

            var h = 0f;
            var with = property.FindPropertyRelative(name);
            if (with != null && with.isArray == true) {

                var size = with.arraySize;
                for (int i = 0; i < size; ++i) {

                    var registry = with.GetArrayElementAtIndex(i);
                    
                    h += FilterDataTypesEditor.lineHeight;
                    if (drawType == ComponentDataTypeAttribute.Type.WithData) {

                        var copy = registry.Copy();
                        var initDepth = copy.depth;
                        if (copy.NextVisible(copy.hasChildren) == true) {
                            do {

                                if (copy.depth <= initDepth) break;
                                h += EditorGUI.GetPropertyHeight(copy, true);

                            } while (copy.NextVisible(false) == true);

                            h += 8f;
                        }

                    } else if (drawType == ComponentDataTypeAttribute.Type.NoData) { }

                }

            }

            return h;

        }

        public override void OnGUI(UnityEngine.Rect position, SerializedProperty property, UnityEngine.GUIContent label) {

            position.height -= marginBottom;
            
            var drawType = ComponentDataTypeAttribute.Type.NoData;
            if (this.GetAttr() is ComponentDataTypeAttribute attr) {

                drawType = attr.type;

            }

            const float pixel = 0.5f;
            const float pixel2 = 1f;
            const float alpha = 0.1f;
            const float alphaBack = 0.02f;
            
            var contentRect = EditorGUI.IndentedRect(position);
            var lineRect = EditorGUI.IndentedRect(position);
            var lineRectLeft = lineRect;
            lineRectLeft.width = pixel;
            lineRectLeft.height -= pixel2;
            lineRectLeft.y += pixel;
            var lineRectRight = lineRect;
            lineRectRight.x += lineRectRight.width;
            lineRectRight.height -= pixel2;
            lineRectRight.y += pixel;
            lineRectRight.width = pixel;
            var lineRectTop = lineRect;
            lineRectTop.height = pixel;
            lineRectTop.width -= pixel2;
            lineRectTop.x += pixel;
            var lineRectBottom = lineRect;
            lineRectBottom.y += lineRectBottom.height;
            lineRectBottom.height = pixel;
            lineRectBottom.width -= pixel2;
            lineRectBottom.x += pixel;
            EditorGUI.DrawRect(lineRectLeft, new Color(1f, 1f, 1f, alpha));
            EditorGUI.DrawRect(lineRectRight, new Color(1f, 1f, 1f, alpha));
            EditorGUI.DrawRect(lineRectTop, new Color(1f, 1f, 1f, alpha));
            EditorGUI.DrawRect(lineRectBottom, new Color(1f, 1f, 1f, alpha));

            contentRect.x += pixel;
            contentRect.width -= pixel2;
            contentRect.y += pixel;
            contentRect.height -= pixel2;
            EditorGUI.DrawRect(contentRect, new Color(1f, 1f, 1f, alphaBack));

            var backRect = EditorGUI.IndentedRect(position);
            position.height = FilterDataTypesEditor.headerHeight;
            EditorGUI.DrawRect(EditorGUI.IndentedRect(position), new Color(1f, 1f, 1f, alphaBack));
            position.x += 8f;
            position.width -= 8f;
            EditorGUI.LabelField(position, label, EditorStyles.boldLabel);
            position.y += FilterDataTypesEditor.headerHeight;
            
            position.height = FilterDataTypesEditor.miniHeight;
            {
                backRect.y = position.y;
                backRect.height = position.height;
                EditorGUI.DrawRect(backRect, new Color(0f, 0f, 0f, 0.1f));
                using (new GUILayoutExt.GUIColorUsing(new Color(1f, 1f, 1f, 0.5f))) {
                    EditorGUI.LabelField(position, "Include:", EditorStyles.miniLabel);
                }
            }

            position.y += FilterDataTypesEditor.miniHeight;
            position = this.DrawArray(position, property, "with", drawType);
            
            position.height = FilterDataTypesEditor.miniHeight;
            {
                backRect.y = position.y;
                backRect.height = position.height;
                EditorGUI.DrawRect(backRect, new Color(0f, 0f, 0f, 0.1f));
                using (new GUILayoutExt.GUIColorUsing(new Color(1f, 1f, 1f, 0.5f))) {
                    EditorGUI.LabelField(position, "Exclude:", EditorStyles.miniLabel);
                }
            }
            position.y += FilterDataTypesEditor.miniHeight;
            position = this.DrawArray(position, property, "without", ComponentDataTypeAttribute.Type.NoData);
            
        }
        
        internal static bool GetTypeFromManagedReferenceFullTypeName(string managedReferenceFullTypename, out System.Type managedReferenceInstanceType)
        {
            managedReferenceInstanceType = null;

            var parts = managedReferenceFullTypename.Split(' ');
            if (parts.Length == 2)
            {
                var assemblyPart = parts[0];
                var nsClassnamePart = parts[1];
                managedReferenceInstanceType = System.Type.GetType($"{nsClassnamePart}, {assemblyPart}");
            }

            return managedReferenceInstanceType != null;
        }

    }

}
