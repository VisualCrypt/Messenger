namespace Obsidian.Language.Strings {
interface IGeneratedResource {
/// <summary>
/// Text like 'Untitled.visualcrypt'.
/// </summary>
string constUntitledDotVisualCrypt { get; }

/// <summary>
/// Text like 'Calculating MAC...'.
/// </summary>
string encProgr_CalculatingMAC { get; }

/// <summary>
/// Text like 'Decrypting MAC...'.
/// </summary>
string encProgr_DecryptingMAC { get; }

/// <summary>
/// Text like 'Decrypting Message...'.
/// </summary>
string encProgr_DecryptingMessage { get; }

/// <summary>
/// Text like 'Decrypting Random Key...'.
/// </summary>
string encProgr_DecryptingRandomKey { get; }

/// <summary>
/// Text like 'Encrypting MAC...'.
/// </summary>
string encProgr_EncryptingMAC { get; }

/// <summary>
/// Text like 'Encrypting Message...'.
/// </summary>
string encProgr_EncryptingMessage { get; }

/// <summary>
/// Text like 'Encrypting Random Key...'.
/// </summary>
string encProgr_EncryptingRandomKey { get; }

/// <summary>
/// Text like 'Processing Key...'.
/// </summary>
string encProgr_ProcessingKey { get; }

/// <summary>
/// Text like 'VisualCrypt {0}, AES 256 Bit, 2^{1} Rounds , {2} Ch.'.
/// </summary>
string encrpytedStatusbarText { get; }

/// <summary>
/// Text like 'Edit'.
/// </summary>
string miEdit { get; }

/// <summary>
/// Text like 'Copy'.
/// </summary>
string miEditCopy { get; }

/// <summary>
/// Text like 'Cut'.
/// </summary>
string miEditCut { get; }

/// <summary>
/// Text like 'Delete Line'.
/// </summary>
string miEditDeleteLine { get; }

/// <summary>
/// Text like 'Find...'.
/// </summary>
string miEditFind { get; }

/// <summary>
/// Text like 'Find Next'.
/// </summary>
string miEditFindNext { get; }

/// <summary>
/// Text like 'Find Previous'.
/// </summary>
string miEditFindPrevious { get; }

/// <summary>
/// Text like 'Go To...'.
/// </summary>
string miEditGoTo { get; }

/// <summary>
/// Text like 'Insert Date, Time'.
/// </summary>
string miEditInsertDateTime { get; }

/// <summary>
/// Text like 'Paste'.
/// </summary>
string miEditPaste { get; }

/// <summary>
/// Text like 'Redo'.
/// </summary>
string miEditRedo { get; }

/// <summary>
/// Text like 'Replace...'.
/// </summary>
string miEditReplace { get; }

/// <summary>
/// Text like 'Select All'.
/// </summary>
string miEditSelectAll { get; }

/// <summary>
/// Text like 'Undo'.
/// </summary>
string miEditUndo { get; }

/// <summary>
/// Text like 'File'.
/// </summary>
string miFile { get; }

/// <summary>
/// Text like 'Exit'.
/// </summary>
string miFileExit { get; }

/// <summary>
/// Text like 'Export Cleartext...'.
/// </summary>
string miFileExportClearText { get; }

/// <summary>
/// Text like 'Import With Encoding...'.
/// </summary>
string miFileImportWithEnconding { get; }

/// <summary>
/// Text like 'New'.
/// </summary>
string miFileNew { get; }

/// <summary>
/// Text like 'Open...'.
/// </summary>
string miFileOpen { get; }

/// <summary>
/// Text like 'Print...'.
/// </summary>
string miFilePrint { get; }

/// <summary>
/// Text like 'Save'.
/// </summary>
string miFileSave { get; }

/// <summary>
/// Text like 'Save As...'.
/// </summary>
string miFileSaveAs { get; }

/// <summary>
/// Text like 'Format'.
/// </summary>
string miFormat { get; }

/// <summary>
/// Text like 'Spellchecking'.
/// </summary>
string miFormatCheckSpelling { get; }

/// <summary>
/// Text like 'Font...'.
/// </summary>
string miFormatFont { get; }

/// <summary>
/// Text like 'Word Wrap'.
/// </summary>
string miFormatWordWrap { get; }

/// <summary>
/// Text like 'Help'.
/// </summary>
string miHelp { get; }

/// <summary>
/// Text like 'About VisualCrypt...'.
/// </summary>
string miHelpAbout { get; }

/// <summary>
/// Text like 'Log...'.
/// </summary>
string miHelpLog { get; }

/// <summary>
/// Text like 'View Help Online'.
/// </summary>
string miHelpViewOnline { get; }

/// <summary>
/// Text like 'VisualCrypt'.
/// </summary>
string miVC { get; }

/// <summary>
/// Text like 'Change Password...'.
/// </summary>
string miVCChangePassword { get; }

/// <summary>
/// Text like 'Decrypt'.
/// </summary>
string miVCDecrypt { get; }

/// <summary>
/// Text like 'Encrypt'.
/// </summary>
string miVCEncrypt { get; }

/// <summary>
/// Text like 'Set Password...'.
/// </summary>
string miVCSetPassword { get; }

/// <summary>
/// Text like 'Cryptography Settings...'.
/// </summary>
string miVCSettings { get; }

/// <summary>
/// Text like 'View'.
/// </summary>
string miView { get; }

/// <summary>
/// Text like 'Language'.
/// </summary>
string miViewLanguage { get; }

/// <summary>
/// Text like 'Status Bar'.
/// </summary>
string miViewStatusBar { get; }

/// <summary>
/// Text like 'Tool Area'.
/// </summary>
string miViewToolArea { get; }

/// <summary>
/// Text like 'Zoom In'.
/// </summary>
string miViewZoomIn { get; }

/// <summary>
/// Text like 'Zoom ({0}%)'.
/// </summary>
string miViewZoomLevelText { get; }

/// <summary>
/// Text like 'Zoom Out'.
/// </summary>
string miViewZoomOut { get; }

/// <summary>
/// Text like 'Incorrect password or corrupted/forged message.'.
/// </summary>
string msgPasswordError { get; }

/// <summary>
/// Text like 'Decrypting:'.
/// </summary>
string operationDecryption { get; }

/// <summary>
/// Text like 'Decrypting loaded file:'.
/// </summary>
string operationDecryptOpenedFile { get; }

/// <summary>
/// Text like 'Encrypting and saving:'.
/// </summary>
string operationEncryptAndSave { get; }

/// <summary>
/// Text like 'Encrypting:'.
/// </summary>
string operationEncryption { get; }

/// <summary>
/// Text like 'Ln {0}, Col {1} | Pos {2}/{3}'.
/// </summary>
string plaintextStatusbarPositionInfo { get; }

/// <summary>
/// Text like 'Plaintext | {0} | {1}'.
/// </summary>
string plaintextStatusbarText { get; }

/// <summary>
/// Text like 'Cancel'.
/// </summary>
string termCancel { get; }

/// <summary>
/// Text like 'Copy to Clipboard'.
/// </summary>
string termCopyToClipboard { get; }

/// <summary>
/// Text like 'Decrypt'.
/// </summary>
string termDecrypt { get; }

/// <summary>
/// Text like 'Encrypt'.
/// </summary>
string termEncrypt { get; }

/// <summary>
/// Text like 'Password'.
/// </summary>
string termPassword { get; }

/// <summary>
/// Text like 'Save'.
/// </summary>
string termSave { get; }

/// <summary>
/// Text like 'Set Password'.
/// </summary>
string termSetPassword { get; }

/// <summary>
/// Text like 'https://visualcrypt.com/category/faq'.
/// </summary>
string uriHelpUrl { get; }

/// <summary>
/// Text like 'https://visualcrypt.com/post/password-quality'.
/// </summary>
string uriPWSpecUrl { get; }

/// <summary>
/// Text like 'https://github.com/VisualCrypt/VisualCrypt'.
/// </summary>
string uriSourceUrl { get; }

/// <summary>
/// Text like 'https://visualcrypt.com/post/visualcrypt-2-encryption-standard-specification'.
/// </summary>
string uriSpecUrl { get; }

/// <summary>
/// Text like 'Discard Changes?'.
/// </summary>
string msgDiscardChanges { get; }

/// <summary>
/// Text like 'This file is neither text nor VisualCrypt!'.
/// </summary>
string msgFileIsBinary { get; }

/// <summary>
/// Text like ''{0}' could not be found.'.
/// </summary>
string msgFindCouldNotBeFound { get; }

/// <summary>
/// Text like 'No match for '{0}' could be found.'.
/// </summary>
string msgFindRegExNoMatch { get; }

/// <summary>
/// Text like 'Nothing found - Search again from the top of the document?'.
/// </summary>
string msgNothingFoundSearchFromStart { get; }

/// <summary>
/// Text like 'Invalid Regular Expression Syntax'.
/// </summary>
string msgReplaceInvalidRegEx { get; }

/// <summary>
/// Text like '{0} occurrences were replaced.'.
/// </summary>
string msgReplaceOccucancesReplaced { get; }

/// <summary>
/// Text like 'Change Password'.
/// </summary>
string spdm_Change_OK { get; }

/// <summary>
/// Text like 'Change Password'.
/// </summary>
string spdm_Change_Title { get; }

/// <summary>
/// Text like 'Change Password and decrypt'.
/// </summary>
string spdm_CorrectPassword_OK { get; }

/// <summary>
/// Text like 'The current password is not correct'.
/// </summary>
string spdm_CorrectPassword_Title { get; }

/// <summary>
/// Text like 'Decrypt loaded file'.
/// </summary>
string spdm_SetAndDecryptLoadedFile_OK { get; }

/// <summary>
/// Text like 'Enter password to decrypt loaded file'.
/// </summary>
string spdm_SetAndDecryptLoadedFile_Title { get; }

/// <summary>
/// Text like 'Decrypt'.
/// </summary>
string spdm_SetAndDecrypt_OK { get; }

/// <summary>
/// Text like 'Set Password & Decrypt'.
/// </summary>
string spdm_SetAndDecrypt_Title { get; }

/// <summary>
/// Text like 'Encrypt and Save'.
/// </summary>
string spdm_SetAndEncryptAndSave_OK { get; }

/// <summary>
/// Text like 'Set Password, Encrypt and Save'.
/// </summary>
string spdm_SetAndEncryptAndSave_Title { get; }

/// <summary>
/// Text like 'Encrypt'.
/// </summary>
string spdm_SetAndEncrypt_OK { get; }

/// <summary>
/// Text like 'Set Password & Encrypt'.
/// </summary>
string spdm_SetAndEncrypt_Title { get; }

/// <summary>
/// Text like 'Set Password'.
/// </summary>
string spdm_Set_OK { get; }

/// <summary>
/// Text like 'Set Password'.
/// </summary>
string spdm_Set_Title { get; }

/// <summary>
/// Text like 'Password/-phrase:'.
/// </summary>
string spd_lbl_PasswordOrPhrase { get; }

/// <summary>
/// Text like 'Generate Password'.
/// </summary>
string spd_linktext_GeneratePassword { get; }

/// <summary>
/// Text like 'Print Password'.
/// </summary>
string spd_linktext_PrintPassword { get; }

/// <summary>
/// Text like 'VisualCrypt Passwords'.
/// </summary>
string spd_linktext_VisualCryptPasswords { get; }

/// <summary>
/// Text like 'The password is effectively empty - are you sure?'.
/// </summary>
string spd_msgPasswordEffectivelyEmpty { get; }

/// <summary>
/// Text like 'Use empty password?'.
/// </summary>
string spd_msgUseEmptyPassword { get; }

/// <summary>
/// Text like '{0} significant Unicode Characters'.
/// </summary>
string spd_msgXofYUnicodeChars { get; }

/// <summary>
/// Text like '(256 Bit Random Data)'.
/// </summary>
string spd_text_from256BitRD { get; }

/// <summary>
/// Text like 'Read more about'.
/// </summary>
string spd_text_ReadMoreAbout { get; }

/// <summary>
/// Text like 'Binary File'.
/// </summary>
string termBinary { get; }

/// <summary>
/// Text like 'Change Password'.
/// </summary>
string termChangePassword { get; }

/// <summary>
/// Text like 'Clear'.
/// </summary>
string termClear { get; }

/// <summary>
/// Text like 'Replace All'.
/// </summary>
string termReplaceAll { get; }

/// <summary>
/// Text like 'Export Clear Text (Encoding: {0})'.
/// </summary>
string titleExportCleartext { get; }

/// <summary>
/// Text like 'Import with encoding: {0}'.
/// </summary>
string titleImportWithEncoding { get; }

/// <summary>
/// Text like 'Font family:'.
/// </summary>
string fnt_labelFontFamilies { get; }

/// <summary>
/// Text like 'Preview:'.
/// </summary>
string fnt_labelPreview { get; }

/// <summary>
/// Text like 'Size:'.
/// </summary>
string fnt_labelSize { get; }

/// <summary>
/// Text like 'Typeface:'.
/// </summary>
string fnt_label_typeFace { get; }

/// <summary>
/// Text like 'The quick brown fox jumps over the lazy dog'.
/// </summary>
string fnt_theQuickBrownFox { get; }

/// <summary>
/// Text like 'VisualCrypt 2 (AES 256 Bit, BCrypt - Multi)'.
/// </summary>
string sett_combo_VisualCrypt2 { get; }

/// <summary>
/// Text like 'source code'.
/// </summary>
string sett_linktext_Source { get; }

/// <summary>
/// Text like 'Spec'.
/// </summary>
string sett_linktext_Spec { get; }

/// <summary>
/// Text like 'BCrypt/AES Rounds:'.
/// </summary>
string sett_text_BCryptAESRounds { get; }

/// <summary>
/// Text like 'Default: 2'.
/// </summary>
string sett_text_default_2_power { get; }

/// <summary>
/// Text like 'Encryption Method:'.
/// </summary>
string sett_text_EncrpytionMethod { get; }

/// <summary>
/// Text like 'or view the'.
/// </summary>
string sett_text_orViewThe { get; }

/// <summary>
/// Text like 'Read the'.
/// </summary>
string sett_text_ReadThe { get; }

/// <summary>
/// Text like 'Warning: A high value will turn encryption and decryption into a very time consuming operation.'.
/// </summary>
string sett_warn_high { get; }

/// <summary>
/// Text like 'Warning: A low value faciliates brute force and dictionary attacks.'.
/// </summary>
string sett_warn_low { get; }

/// <summary>
/// Text like 'The setting influences the required computational work to create the BCrypt hash. A higher value means more work.'.
/// </summary>
string sett_warn_neutral { get; }

/// <summary>
/// Text like 'Defaults'.
/// </summary>
string termDefaults { get; }

/// <summary>
/// Text like 'OK'.
/// </summary>
string termOK { get; }

/// <summary>
/// Text like 'Clear Password'.
/// </summary>
string termClearPassword { get; }

/// <summary>
/// Text like 'Invalid VisualCrypt format - Cannot decrypt this.'.
/// </summary>
string msgFormatError { get; }

/// <summary>
/// Text like 'Invalid Filename.'.
/// </summary>
string msgInvalidFilename { get; }

/// <summary>
/// Text like 'This debug information will not be saved to a file.'.
/// </summary>
string logWindowInfoText { get; }

/// <summary>
/// Text like 'Close'.
/// </summary>
string termClose { get; }

/// <summary>
/// Text like 'Find'.
/// </summary>
string termFind { get; }

/// <summary>
/// Text like 'Go'.
/// </summary>
string termGo { get; }

/// <summary>
/// Text like 'Replace'.
/// </summary>
string termReplace { get; }

/// <summary>
/// Text like 'Line Number (1-'.
/// </summary>
string toolAreaGoLineNo { get; }

/// <summary>
/// Text like 'Match case'.
/// </summary>
string toolAreaMatchCase { get; }

/// <summary>
/// Text like 'Match whole word'.
/// </summary>
string toolAreaMatchWholeWord { get; }

/// <summary>
/// Text like 'Use Regular Expressions'.
/// </summary>
string toolAreaRegularEx { get; }

/// <summary>
/// Text like 'Replace with:'.
/// </summary>
string toolAreaReplaceWith { get; }

/// <summary>
/// Text like 'Search up'.
/// </summary>
string toolAreaSearchUp { get; }

/// <summary>
/// Text like 'Notes'.
/// </summary>
string termNotes { get; }

/// <summary>
/// Text like 'Delete Note?'.
/// </summary>
string fileDlgDelete { get; }

/// <summary>
/// Text like 'Delete Notes?'.
/// </summary>
string fileDlgDeleteMany { get; }

/// <summary>
/// Text like 'Rename Note'.
/// </summary>
string fileDlgRename { get; }

/// <summary>
/// Text like 'Save Note'.
/// </summary>
string fileDlgSetName { get; }

/// <summary>
/// Text like 'Filename:'.
/// </summary>
string termFilename { get; }

/// <summary>
/// Text like 'Share'.
/// </summary>
string termShare { get; }

/// <summary>
/// Text like 'Loading File:'.
/// </summary>
string fileProgr_Loading { get; }

/// <summary>
/// Text like 'Saving File:'.
/// </summary>
string fileProgr_Saving { get; }

/// <summary>
/// Text like 'Analyzing Contents...'.
/// </summary>
string fileProg_MsgAnalyzingContents { get; }

/// <summary>
/// Text like 'If this takes too long on your device, choose less Rounds in Settings.'.
/// </summary>
string msgTakesTooLong { get; }

}
}
