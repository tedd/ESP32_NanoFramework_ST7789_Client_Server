ffmpeg -i ../rick.mkv -vf 'crop=ih:ih' -codec:v mpeg2video -an ../Rick_Crop.mpg
ffmpeg -i ../Rick_Crop.mpeg -vf scale=w=128:h=128:force_original_aspect_ratio=decrease -codec:v mpeg2video -an ../Rick_128.mpg 
ffmpeg -i ../Rick_128.mpg -vf :scale=w=128:h=128:force_original_aspect_ratio=decrease -an -pixel_format rgb565 -vf fps=10 out%04d.png 
