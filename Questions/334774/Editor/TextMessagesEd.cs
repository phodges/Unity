using UnityEngine;
using UnityEditor;

/*
Copyright (c) 2012, Peter Hodges
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met: 

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer. 
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies, 
either expressed or implied, of the FreeBSD Project.
*/

/// <summary>
/// A custom editor window, providing the means to add and remove text messages from
/// objects of the TextMessages class.
/// </summary>
public class TextMessagesEd : EditorWindow {

  private TextMessages _current;
  private SerializedObject _serializedObject;
  private SerializedProperty _serializedProperty;
  private string _message;

  [MenuItem("draitch/Questions/TextMessageEd")]
  static void Init()
  {
    EditorWindow.GetWindow<TextMessagesEd>("Text Messages");
  }

  /// <summary>
  /// Upon receiving focus, we ensure that the window's selection is up to date.
  /// </summary>
  void OnFocus()
  {
    SetSelection();
  }

  /// <summary>
  /// Update state in response to the user changing their current selection
  /// </summary>
  void OnSelectionChange()
  {
    SetSelection();
  }

  /// <summary>
  /// Attempts to find a TextMessages component within the current selection,
  /// binding this window to the first such item discovered.
  /// </summary>
  void SetSelection()
  {
    TextMessages discovered = null;
    // Work with the first discovered object
    Object[] filtered = Selection.GetFiltered(typeof(TextMessages), SelectionMode.Editable);
    if (null != filtered && 0 < filtered.Length)
      {
	// Accept the first result
	discovered = filtered[0] as TextMessages;
      }
    BindProperty(discovered);
    Repaint();
  }

  /// <summary>
  /// Drives the GUI.
  /// </summary>
  void OnGUI()
  {
    if (null == _current)
      {
	EditorGUILayout.LabelField("Please select a TextMessage object");
      }
    else
      {
	_serializedObject.Update();
	EditorGUILayout.LabelField("Editing " + _current.name);
	EditorGUILayout.Space();
        PresentExistingMessages();
	EditorGUILayout.Space();
        PresentAddMessage();
	EditorGUILayout.Space();
	if (GUILayout.Button("Close"))
	  {
	    Close();
	  }
      }
  }

  /// <summary>
  /// Shows all messages currently contained within the TextMessage.
  /// </summary>
  bool PresentExistingMessages()
  {
    bool modified = false;
    int removeIndex = -1;
    for (int i=0; i<_serializedProperty.arraySize; ++i)
      {
	if (PresentMessage(_current.messages[i]))
	  {
	    removeIndex = i;
	  }
      }

    if (-1 < removeIndex)
      {
	_serializedProperty.DeleteArrayElementAtIndex(removeIndex);
	_serializedObject.ApplyModifiedProperties();
	modified = true;
      }
    return modified;
  }

  /// <summary>
  /// Shows a particular message, alongside a delete button.
  /// </summary>
  bool PresentMessage(string msg)
  {
    bool remove;
    EditorGUILayout.BeginHorizontal();
    remove = GUILayout.Button("-", GUILayout.MaxWidth(16));
    EditorGUILayout.LabelField(msg);
    EditorGUILayout.EndHorizontal();
    return remove;
  }

  /// <summary>
  /// Shows the GUI to add new text to the current TextMessage
  /// </summary>
  bool PresentAddMessage()
  {
    bool modified = false;
    EditorGUILayout.LabelField("Enter new message:");
    string content = string.IsNullOrEmpty(_message) ? "Type here" : _message;
    _message = EditorGUILayout.TextArea(content);
    if (!string.IsNullOrEmpty(_message) && GUILayout.Button("Add"))
      {
	int enlarged = _serializedProperty.arraySize;
	_serializedProperty.InsertArrayElementAtIndex(enlarged);
	_serializedObject.ApplyModifiedProperties();
	_current.messages[enlarged] = _message;
	_message = null;
	modified = true;
      }
    return modified;
  }

  /// <summary>
  /// Associates the window with the specified TextMessage, creating an appropriate
  /// Serialized Object and Property, if non-null and clearing values otherwise.
  /// </summary>
  void BindProperty(TextMessages bind)
  {
    if (_current != bind)
      {
	if (null != bind)
	  {
	    _serializedObject = new SerializedObject(bind);
	    _serializedProperty = _serializedObject.FindProperty("messages");
	  }
	else
	  {
	    _serializedObject = null;
	    _serializedProperty = null;
	  }
	_current = bind;
      }
  }

}
