# MouseTests

This a test program to prove the mouse issue with `Windows Terminal` by not send mouse events when the mouse is pressed and moving outside at top of the WT, no mater I'm using win32-input-mode or using virtual terminal sequences. It only send events inside and outside left, bottom and right, but not when it's outside of the top window.

![MouseTests](MouseTests.gif)

This a test program to prove the mouse issue with `Windows Terminal` by not send mouse events when the mouse is pressed and moving outside at right or left of the WT in the same row, no mater I'm using win32-input-mode or using virtual terminal sequences. It only send events inside and outside left or right when moving up or down, but not when moving right and left at the same row.

![MouseTests](MouseTests2.gif)


With the Windows Host Console this doesn't happens, so I imagine that it's only a WT issue. Thanks for your attention.