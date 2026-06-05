package com.backrooms.levelzero;

import android.app.Activity;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.content.Context;
import android.content.SharedPreferences;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.LinearGradient;
import android.graphics.Paint;
import android.graphics.Path;
import android.graphics.RadialGradient;
import android.graphics.RectF;
import android.graphics.Shader;
import android.graphics.Typeface;
import android.media.AudioAttributes;
import android.media.AudioFormat;
import android.media.AudioTrack;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Locale;
import java.util.Random;
import java.util.Stack;

public class MainActivity extends Activity {
    Game g;
    Sound s;

    @Override public void onCreate(Bundle b) {
        super.onCreate(b);
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, WindowManager.LayoutParams.FLAG_FULLSCREEN);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
        g = new Game(this);
        s = new Sound(g);
        setContentView(g);
    }

    @Override public void onResume() {
        super.onResume();
        g.run = true;
        s.start();
    }

    @Override public void onPause() {
        g.run = false;
        s.stop();
        super.onPause();
    }

    @Override public void onBackPressed() {
        g.back();
    }

    static class Game extends View {
        static final int MENU = 0, LOADING = 1, PLAY = 2, PAUSE = 3, SETTINGS = 4, CONTROLS = 5, WIN = 6;
        Paint p = new Paint(Paint.ANTI_ALIAS_FLAG);
        Random r = new Random(7);
        boolean run = true, sprint = false, hunt = false, usePulse = false;
        int screen = MENU, previousScreen = MENU, W = 51, H = 51, lp = -1, rp = -1, sp = -1, up = -1, dragUi = 0;
        int q = 1, frameCap = 60, found = 0;
        float x = 2.5f, y = 2.5f, a = .2f, sen = 1, brightness = 1, volume = 1, graphicsScale = 1;
        float controlOpacity = .78f, controlScale = 1, joyx, joyy, jx, jy, mx, my, ld, bob, stam = 1;
        float ex = 48.5f, ey = 48.5f, monx = 44.5f, mony = 43.5f, noise = 0, load = 0;
        long last = System.nanoTime();
        boolean[][] open = new boolean[W][H], lite = new boolean[W][H], btn = new boolean[W][H], pit = new boolean[W][H], chair = new boolean[W][H];

        Game(Context c) {
            super(c);
            setFocusable(true);
            loadSettings();
            gen();
        }

        void loadSettings() {
            SharedPreferences s = getContext().getSharedPreferences("s", 0);
            sen = s.getFloat("sen", 1);
            q = s.getInt("q", 1);
            brightness = s.getFloat("brightness", 1);
            volume = s.getFloat("volume", 1);
            frameCap = s.getInt("frameCap", 60);
            graphicsScale = s.getFloat("graphicsScale", 1);
            controlOpacity = s.getFloat("controlOpacity", .78f);
            controlScale = s.getFloat("controlScale", 1);
        }

        void save() {
            getContext().getSharedPreferences("s", 0).edit()
                    .putFloat("sen", sen)
                    .putInt("q", q)
                    .putFloat("brightness", brightness)
                    .putFloat("volume", volume)
                    .putInt("frameCap", frameCap)
                    .putFloat("graphicsScale", graphicsScale)
                    .putFloat("controlOpacity", controlOpacity)
                    .putFloat("controlScale", controlScale)
                    .apply();
        }

        void newRun() {
            x = 2.5f; y = 2.5f; a = .2f; monx = 44.5f; mony = 43.5f; found = 0; noise = 0; stam = 1;
            sprint = false; hunt = false; usePulse = false; mx = my = ld = 0; lp = rp = sp = up = -1;
            gen();
        }

        void gen() {
            r = new Random(7);
            open = new boolean[W][H]; lite = new boolean[W][H]; btn = new boolean[W][H]; pit = new boolean[W][H]; chair = new boolean[W][H];
            boolean[][] v = new boolean[W][H];
            Stack<int[]> st = new Stack<>();
            st.push(new int[]{1, 1}); v[1][1] = open[1][1] = true;
            int[][] D = {{2,0},{-2,0},{0,2},{0,-2}};
            while (!st.empty()) {
                int[] c = st.peek();
                ArrayList<int[]> ds = new ArrayList<>();
                for (int[] d : D) {
                    int nx = c[0] + d[0], ny = c[1] + d[1];
                    if (nx > 0 && ny > 0 && nx < W - 1 && ny < H - 1 && !v[nx][ny]) ds.add(d);
                }
                if (ds.isEmpty()) st.pop(); else {
                    int[] d = ds.get(r.nextInt(ds.size()));
                    open[c[0] + d[0] / 2][c[1] + d[1] / 2] = true;
                    open[c[0] + d[0]][c[1] + d[1]] = true;
                    v[c[0] + d[0]][c[1] + d[1]] = true;
                    st.push(new int[]{c[0] + d[0], c[1] + d[1]});
                }
            }
            for (int i = 0; i < 340; i++) {
                int xx = 1 + r.nextInt(W - 3), yy = 1 + r.nextInt(H - 3), ww = 1 + r.nextInt(5), hh = 1 + r.nextInt(4);
                for (int ox = 0; ox < ww && xx + ox < W - 1; ox++) for (int oy = 0; oy < hh && yy + oy < H - 1; oy++) open[xx + ox][yy + oy] = true;
            }
            for (int i = 1; i < W - 1; i++) for (int j = 1; j < H - 1; j++) if (open[i][j]) {
                lite[i][j] = (i * 7 + j * 13) % 5 == 0;
                pit[i][j] = r.nextFloat() < .012f && dist(i, j, x, y) > 12;
                chair[i][j] = r.nextFloat() < .018f && dist(i, j, x, y) > 9;
            }
            place(12, 12); place(28, 30); place(43, 42);
            open[48][48] = open[47][48] = open[48][47] = lite[48][48] = true;
        }

        void place(int bx, int by) {
            for (int rr = 0; rr < 9; rr++) for (int i = Math.max(1, bx - rr); i < Math.min(W - 1, bx + rr); i++) for (int j = Math.max(1, by - rr); j < Math.min(H - 1, by + rr); j++) if (open[i][j]) { btn[i][j] = lite[i][j] = true; return; }
        }

        @Override protected void onDraw(Canvas c) {
            int w = getWidth(), h = getHeight();
            long n = System.nanoTime();
            float dt = Math.min(.05f, (n - last) / 1e9f);
            last = n;
            if (screen == PLAY) {
                step(dt);
                drawWorld(c, w, h);
                hud(c, w, h);
            } else if (screen == PAUSE) {
                drawWorld(c, w, h);
                hud(c, w, h);
                drawPause(c, w, h);
            } else if (screen == MENU) {
                drawMenu(c, w, h);
            } else if (screen == LOADING) {
                load += dt;
                drawLoading(c, w, h);
                if (load > 1.35f) screen = PLAY;
            } else if (screen == SETTINGS) {
                drawSettings(c, w, h);
            } else if (screen == CONTROLS) {
                drawControlsEditor(c, w, h);
            } else if (screen == WIN) {
                drawWorld(c, w, h);
                drawWin(c, w, h);
            }
            scheduleNextFrame();
        }

        void scheduleNextFrame() {
            if (!run) return;
            if (frameCap <= 30) postInvalidateDelayed(33);
            else if (frameCap <= 45) postInvalidateDelayed(22);
            else postInvalidateOnAnimation();
        }

        void step(float dt) {
            a += ld * .0048f * sen; ld = 0;
            float mv = Math.min(1, Math.abs(mx) + Math.abs(my));
            if (sprint && mv > .1f && stam > .04f) stam = Math.max(0, stam - dt * .25f); else stam = Math.min(1, stam + dt * .17f);
            float spd = (sprint && stam > .04f ? 4.6f : 2.45f) * dt;
            float fx = (float)Math.cos(a), fy = (float)Math.sin(a), rx = -fy, ry = fx;
            float dx = (fx * my + rx * mx) * spd, dy = (fy * my + ry * mx) * spd;
            if (walk(x + dx, y)) x += dx; if (walk(x, y + dy)) y += dy;
            if (mv > .05f) bob += dt * (sprint ? 10 : 6);
            monster(dt);
            if (usePulse) { useNearby(); usePulse = false; }
            if (found >= 3 && dist(x, y, ex, ey) < 1.2f) screen = WIN;
        }

        void useNearby() {
            for (int i = Math.max(1, (int)x - 1); i <= Math.min(W - 2, (int)x + 1); i++) for (int j = Math.max(1, (int)y - 1); j <= Math.min(H - 2, (int)y + 1); j++) if (btn[i][j] && dist(x, y, i + .5f, j + .5f) < 1.55f) { btn[i][j] = false; found++; noise = 1; }
        }

        void monster(float dt) {
            float d = dist(x, y, monx, mony);
            noise = Math.max(0, noise - dt * .15f);
            hunt = d < (sprint ? 14 : 8) || noise > .4f;
            float tx = hunt ? x : monx + (float)Math.cos(bob * .27f), ty = hunt ? y : mony + (float)Math.sin(bob * .21f);
            float dx = tx - monx, dy = ty - mony, l = (float)Math.hypot(dx, dy);
            if (l > .05f) {
                dx /= l; dy /= l;
                float s = (hunt ? 1.9f : .7f) * dt;
                if (walk(monx + dx * s, mony)) monx += dx * s;
                if (walk(monx, mony + dy * s)) mony += dy * s;
            }
            if (d < 1) { x = 2.5f; y = 2.5f; a = .2f; monx = 44.5f; mony = 43.5f; noise = 1; }
        }

        boolean walk(float xx, float yy) {
            int i = (int)xx, j = (int)yy;
            return i > 0 && j > 0 && i < W - 1 && j < H - 1 && open[i][j] && !pit[i][j];
        }

        float dist(float ax, float ay, float bx, float by) { return (float)Math.hypot(ax - bx, ay - by); }

        void drawBackroomsBackdrop(Canvas c, int w, int h, float dark) {
            p.setStyle(Paint.Style.FILL);
            p.setShader(new LinearGradient(0, 0, 0, h, 0xff191611, 0xff8e7a3d, Shader.TileMode.CLAMP)); c.drawRect(0, 0, w, h, p); p.setShader(null);
            p.setColor(0x55201810); c.drawRect(0, h * .56f, w, h, p);
            for (int i = 0; i < 10; i++) {
                float x0 = w * (i / 10f), x1 = w * ((i + 1) / 10f);
                p.setColor(i % 2 == 0 ? 0x22fff0a0 : 0x11907030); c.drawRect(x0, 0, x1, h * .55f, p);
            }
            for (int i = 0; i < 7; i++) {
                float lx = w * (i + .5f) / 7f;
                p.setShader(new RadialGradient(lx, h * .18f, w * .16f, 0x77fff6ad, 0x00fff6ad, Shader.TileMode.CLAMP)); c.drawCircle(lx, h * .18f, w * .16f, p); p.setShader(null);
                p.setColor(0xddfff2a0); c.drawRoundRect(new RectF(lx - w * .035f, h * .12f, lx + w * .035f, h * .14f), 6, 6, p);
            }
            p.setColor(0x774f3b1c);
            for (int i = 0; i < 28; i++) c.drawLine(w * .15f + i * w * .035f, h, w * .45f + i * w * .01f, h * .56f, p);
            p.setShader(new RadialGradient(w / 2f, h / 2f, Math.max(w, h) * .85f, 0x00000000, Color.argb((int)(dark * 220), 0, 0, 0), Shader.TileMode.CLAMP)); c.drawRect(0, 0, w, h, p); p.setShader(null);
        }

        void drawMenu(Canvas c, int w, int h) {
            drawBackroomsBackdrop(c, w, h, .75f);
            p.setTextAlign(Paint.Align.CENTER); p.setTypeface(Typeface.DEFAULT_BOLD);
            p.setColor(0x33101008); p.setTextSize(h * .095f); c.drawText("BACKROOMS", w / 2f + 3, h * .23f + 4, p);
            p.setColor(0xffffe28a); c.drawText("BACKROOMS", w / 2f, h * .23f, p);
            p.setTextSize(h * .052f); p.setColor(0xffd3bd65); c.drawText("LEVEL ZERO", w / 2f, h * .305f, p);
            menuButton(c, menuEnter(w,h), "ENTER LEVEL 0", false);
            menuButton(c, menuSettings(w,h), "SETTINGS", false);
            menuButton(c, menuExit(w,h), "EXIT", false);
            p.setTextSize(h * .026f); p.setTypeface(Typeface.DEFAULT); p.setColor(0x99fff0a0); c.drawText("damp carpet • fluorescent hum • no clipping out", w / 2f, h * .88f, p);
            p.setTextAlign(Paint.Align.LEFT);
        }

        void drawLoading(Canvas c, int w, int h) {
            drawBackroomsBackdrop(c, w, h, .88f);
            float progress = Math.min(1, load / 1.35f);
            p.setTextAlign(Paint.Align.CENTER); p.setTypeface(Typeface.DEFAULT_BOLD); p.setTextSize(h * .05f); p.setColor(0xffffe28a);
            c.drawText("LOADING FLUORESCENT MAZE", w / 2f, h * .48f, p);
            RectF bar = new RectF(w * .28f, h * .55f, w * .72f, h * .585f);
            p.setColor(0x553d321d); c.drawRoundRect(bar, 10, 10, p);
            RectF fill = new RectF(bar.left, bar.top, bar.left + bar.width() * progress, bar.bottom);
            p.setColor(0xccfff0a0); c.drawRoundRect(fill, 10, 10, p);
            p.setColor(0x66fff6ba);
            for (int i = 0; i < 6; i++) if ((i + (int)(load * 12)) % 3 != 0) c.drawRoundRect(new RectF(bar.left + i * bar.width() / 6f + 3, bar.top + 3, bar.left + (i + .8f) * bar.width() / 6f, bar.bottom - 3), 8, 8, p);
            p.setTextSize(h * .025f); p.setTypeface(Typeface.DEFAULT); p.setColor(0xaae8d78a); c.drawText("Stay quiet. The carpet remembers footsteps.", w / 2f, h * .65f, p);
            p.setTextAlign(Paint.Align.LEFT);
        }

        void drawWorld(Canvas c, int w, int h) {
            float b = .7f + brightness * .3f;
            p.setStyle(Paint.Style.FILL); p.setShader(new LinearGradient(0,0,0,h/2, mul(0xff5a5432,b), mul(0xffb8a85a,b), Shader.TileMode.CLAMP)); c.drawRect(0,0,w,h/2,p);
            p.setShader(new LinearGradient(0,h/2,0,h, mul(0xff756739,b), mul(0xff302818,b), Shader.TileMode.CLAMP)); c.drawRect(0,h/2,w,h,p); p.setShader(null);
            int cols = (int)((q == 0 ? 140 : q == 1 ? 220 : 300) * graphicsScale); cols = Math.max(100, Math.min(360, cols));
            float[] z = new float[cols];
            for (int i = 0; i < cols; i++) {
                float ra = a - .6f + 1.2f * i / cols;
                Ray ray = cast(ra); float d = Math.max(.08f, ray.d * (float)Math.cos(ra - a)); z[i] = d;
                int sh = (int)(h / (d * .72f)), top = h / 2 - sh / 2 + (int)(Math.sin(bob) * 8), bot = h / 2 + sh / 2;
                float fog = Math.min(1, d / (q == 0 ? 13 : 22));
                int rr = lerp(ray.side ? 158 : 186, 72, fog), gg = lerp(ray.side ? 148 : 170, 68, fog), bb = lerp(ray.side ? 70 : 86, 42, fog);
                if (ray.light) { rr += 20; gg += 18; }
                p.setColor(Color.rgb(cl((int)(rr*b)), cl((int)(gg*b)), cl((int)(bb*b))));
                float sx = i * w / (float)cols, sw = w / (float)cols + 1;
                c.drawRect(sx, top, sx + sw, bot, p);
                if (q > 0 && d < 16 && ((int)(ray.hx * 5 + ray.hy * 9)) % 10 == 0) { p.setColor(0x22332f18); c.drawRect(sx, top, sx + sw, bot, p); }
                if (ray.light && d < 11) { p.setColor(0x88fff2a0); c.drawRect(sx, top - 8, sx + sw, top + 4, p); }
            }
            ArrayList<Spr> ss = sprites(); Collections.sort(ss, (u, v) -> Float.compare(v.d, u.d)); for (Spr s : ss) spr(c, w, h, cols, z, s);
            hands(c, w, h); post(c, w, h);
        }

        Ray cast(float ra) {
            float dx = (float)Math.cos(ra), dy = (float)Math.sin(ra); Ray o = new Ray();
            for (float t = .04f; t < 40; t += .04f) {
                float hx = x + dx * t, hy = y + dy * t; int i = (int)hx, j = (int)hy;
                if (i < 0 || j < 0 || i >= W || j >= H || !open[i][j]) { o.d = t; o.hx = hx; o.hy = hy; o.side = Math.abs(hx - i - .5f) > Math.abs(hy - j - .5f); o.light = ((Math.max(0, Math.min(W - 1, i)) + Math.max(0, Math.min(H - 1, j))) % 5) == 0; return o; }
            }
            o.d = 40; return o;
        }

        ArrayList<Spr> sprites() {
            ArrayList<Spr> s = new ArrayList<>();
            for (int i = 1; i < W - 1; i++) for (int j = 1; j < H - 1; j++) if (open[i][j]) {
                float d = dist(x, y, i + .5f, j + .5f);
                if (d < 22 && d > .5f) { if (btn[i][j]) s.add(new Spr(i + .5f, j + .5f, 0, d)); if (chair[i][j]) s.add(new Spr(i + .5f, j + .5f, 1, d)); if (pit[i][j]) s.add(new Spr(i + .5f, j + .5f, 2, d)); }
            }
            s.add(new Spr(monx, mony, 3, dist(x, y, monx, mony))); s.add(new Spr(ex, ey, 4, dist(x, y, ex, ey))); return s;
        }

        void spr(Canvas c, int w, int h, int cols, float[] z, Spr s) {
            float dx = s.x - x, dy = s.y - y, d = (float)Math.hypot(dx, dy), rel = norm((float)Math.atan2(dy, dx) - a);
            if (Math.abs(rel) > .72f || d < .25f) return;
            int sx = (int)((rel / 1.2f + .5f) * w), col = Math.max(0, Math.min(cols - 1, (int)(sx / (float)w * cols)));
            if (d > z[col] + .4f) return;
            int size = (int)(h / (d * .85f)), cy = h / 2 + size / 6 + (int)(Math.sin(bob) * 8);
            if (s.t == 0) { p.setColor(0xddff251a); c.drawCircle(sx, cy, size * .13f, p); p.setColor(0x77ffe68a); c.drawCircle(sx, cy, size * .25f, p); }
            else if (s.t == 1) { p.setColor(0xffdad4b8); c.drawRect(sx - size * .18f, cy - size * .25f, sx + size * .18f, cy + size * .1f, p); p.setStrokeWidth(Math.max(2, size * .035f)); c.drawLine(sx - size * .13f, cy + size * .1f, sx - size * .23f, cy + size * .42f, p); c.drawLine(sx + size * .13f, cy + size * .1f, sx + size * .23f, cy + size * .42f, p); }
            else if (s.t == 2) { p.setColor(0xff141008); c.drawOval(new RectF(sx - size * .5f, cy + size * .2f, sx + size * .5f, cy + size * .55f), p); }
            else if (s.t == 4) { p.setColor(found >= 3 ? 0xff7a351d : 0xff231511); c.drawRect(sx - size * .22f, cy - size * .55f, sx + size * .22f, cy + size * .36f, p); p.setColor(found >= 3 ? 0xffffe08a : 0xff473127); c.drawCircle(sx + size * .12f, cy - size * .05f, Math.max(3, size * .035f), p); }
            else { p.setColor(Color.argb(cl((int)(245 - d * 5)), 5, 4, 3)); c.drawOval(new RectF(sx - size * .16f, cy - size * .6f, sx + size * .16f, cy + size * .35f), p); c.drawCircle(sx, cy - size * .73f, size * .18f, p); p.setStrokeWidth(Math.max(3, size * .045f)); p.setStrokeCap(Paint.Cap.ROUND); c.drawLine(sx - size * .1f, cy - size * .25f, sx - size * .42f, cy + size * .22f, p); c.drawLine(sx + size * .1f, cy - size * .25f, sx + size * .42f, cy + size * .22f, p); c.drawLine(sx - size * .05f, cy + size * .25f, sx - size * .24f, cy + size * .72f, p); c.drawLine(sx + size * .05f, cy + size * .25f, sx + size * .24f, cy + size * .72f, p); p.setColor(0xccffe875); c.drawCircle(sx - size * .06f, cy - size * .75f, Math.max(2, size * .025f), p); c.drawCircle(sx + size * .06f, cy - size * .75f, Math.max(2, size * .025f), p); }
        }

        void hands(Canvas c, int w, int h) {
            p.setStyle(Paint.Style.FILL); p.setColor(0xffd6c64e);
            Path L = new Path(); L.moveTo(w * .30f, h); L.cubicTo(w * .33f, h * .80f, w * .43f, h * .78f, w * .46f, h); L.close(); c.drawPath(L, p);
            Path R = new Path(); R.moveTo(w * .70f, h); R.cubicTo(w * .67f, h * .80f, w * .57f, h * .78f, w * .54f, h); R.close(); c.drawPath(R, p);
            p.setColor(0xff16120d); c.drawOval(new RectF(w * .42f, h * .88f, w * .49f, h * 1.03f), p); c.drawOval(new RectF(w * .51f, h * .88f, w * .58f, h * 1.03f), p);
        }

        void post(Canvas c, int w, int h) {
            if (q > 0) { p.setColor(hunt ? 0x223a0000 : 0x11000000); for (int i = 0; i < 70; i++) { float gx = r.nextFloat() * w, gy = r.nextFloat() * h; c.drawRect(gx, gy, gx + 2, gy + 1, p); } }
            p.setShader(new RadialGradient(w / 2f, h / 2f, Math.max(w, h) * .78f, 0x00000000, hunt ? 0xcc000000 : 0x99000000, Shader.TileMode.CLAMP)); c.drawRect(0, 0, w, h, p); p.setShader(null);
        }

        void hud(Canvas c, int w, int h) {
            p.setTextAlign(Paint.Align.LEFT); p.setTypeface(Typeface.DEFAULT_BOLD); p.setTextSize(h * .04f); p.setColor(0xffffe88a);
            String o = found >= 3 ? "Objective: reach the strange exit door" : "Objective: use red buttons " + found + "/3";
            c.drawText(o, w * .035f, h * .08f, p);
            p.setTypeface(Typeface.DEFAULT); p.setTextSize(h * .028f); p.setColor(hunt ? 0xffff9999 : 0xccffe88a); c.drawText(hunt ? "Something heard you." : "Level 0 / The Lobby", w * .035f, h * .125f, p);
            controls(c, w, h);
            p.setColor(0x663d321d); c.drawRoundRect(new RectF(w * .035f, h * .91f, w * .31f, h * .93f), 8, 8, p);
            p.setColor(stam > .25f ? 0xccffe477 : 0xccff805c); c.drawRoundRect(new RectF(w * .035f, h * .91f, w * (.035f + .275f * stam), h * .93f), 8, 8, p);
            p.setColor(0x663d321d); c.drawRoundRect(new RectF(w * .035f, h * .94f, w * .31f, h * .96f), 8, 8, p);
            p.setColor(hunt ? 0xccff7777 : 0xccd8cc70); c.drawRoundRect(new RectF(w * .035f, h * .94f, w * (.31f - .18f * (hunt ? .2f : 0)), h * .96f), 8, 8, p);
            p.setTextSize(h * .021f); p.setColor(0x99ffe88a); c.drawText("STAMINA", w * .035f, h * .895f, p); c.drawText("SANITY", w * .035f, h * .985f, p);
        }

        void controls(Canvas c, int w, int h) {
            if (joyx == 0) { joyx = w * .15f; joyy = h * .75f; jx = joyx; jy = joyy; }
            int base = Color.argb((int)(controlOpacity * 64), 255, 240, 160), knob = Color.argb((int)(controlOpacity * 144), 255, 240, 160);
            float js = h * .13f * controlScale;
            p.setColor(base); c.drawCircle(joyx, joyy, js, p); p.setColor(knob); c.drawCircle(jx, jy, js * .40f, p);
            menuButton(c, useR(w,h), "USE", false);
            RectF sr = sprintR(w,h); p.setColor(sprint ? 0x99ffe477 : Color.argb((int)(controlOpacity * 92), 255, 228, 119)); c.drawOval(sr, p); label(c, sr, "SPRINT", h * .028f);
            RectF pr = pauseR(w,h); p.setColor(Color.argb((int)(controlOpacity * 88), 255, 240, 160)); c.drawRoundRect(pr, 10, 10, p); label(c, pr, "Ⅱ", h * .055f);
        }

        void drawPause(Canvas c, int w, int h) {
            p.setColor(0xd9000000); c.drawRect(0, 0, w, h, p);
            RectF panel = new RectF(w * .26f, h * .13f, w * .74f, h * .87f); p.setColor(0xee2b281a); c.drawRoundRect(panel, 18, 18, p); p.setStyle(Paint.Style.STROKE); p.setStrokeWidth(2); p.setColor(0x88f1dc82); c.drawRoundRect(panel, 18, 18, p); p.setStyle(Paint.Style.FILL);
            p.setTextAlign(Paint.Align.CENTER); p.setColor(0xffffe28a); p.setTypeface(Typeface.DEFAULT_BOLD); p.setTextSize(h * .06f); c.drawText("PAUSED", w / 2f, h * .255f, p);
            menuButton(c, pauseResume(w,h), "RESUME", false); menuButton(c, pauseSettings(w,h), "SETTINGS", false); menuButton(c, pauseControls(w,h), "CONTROL LAYOUT", false); menuButton(c, pauseExit(w,h), "EXIT GAME", false);
            p.setTextAlign(Paint.Align.LEFT);
        }

        void drawSettings(Canvas c, int w, int h) {
            drawBackroomsBackdrop(c, w, h, .84f);
            RectF panel = new RectF(w * .16f, h * .09f, w * .84f, h * .91f); p.setColor(0xee221f15); c.drawRoundRect(panel, 20, 20, p); p.setStyle(Paint.Style.STROKE); p.setStrokeWidth(2); p.setColor(0x99f1dc82); c.drawRoundRect(panel, 20, 20, p); p.setStyle(Paint.Style.FILL);
            p.setTextAlign(Paint.Align.CENTER); p.setTypeface(Typeface.DEFAULT_BOLD); p.setTextSize(h * .052f); p.setColor(0xffffe28a); c.drawText("SETTINGS", w / 2f, h * .18f, p);
            slider(c, w, h, 0, "Sensitivity", sen, .5f, 2.4f, String.format(Locale.US, "%.1f", sen));
            slider(c, w, h, 1, "Brightness", brightness, .65f, 1.35f, pct(brightness, .65f, 1.35f));
            slider(c, w, h, 2, "Volume", volume, 0f, 1f, pct(volume, 0f, 1f));
            slider(c, w, h, 3, "Graphics Scale", graphicsScale, .7f, 1.2f, pct(graphicsScale, .7f, 1.2f));
            menuButton(c, settingsFrame(w,h), "FRAME CAP: " + frameCap, false); menuButton(c, settingsControls(w,h), "CONTROL LAYOUT", false); menuButton(c, settingsBack(w,h), "BACK", false);
            p.setTextAlign(Paint.Align.LEFT);
        }

        void slider(Canvas c, int w, int h, int row, String name, float value, float min, float max, String show) {
            float y = h * (.265f + row * .095f);
            p.setTextAlign(Paint.Align.LEFT); p.setTypeface(Typeface.DEFAULT_BOLD); p.setTextSize(h * .03f); p.setColor(0xfff1dc82); c.drawText(name, w * .23f, y, p);
            RectF track = new RectF(w * .43f, y - h * .022f, w * .72f, y - h * .002f); p.setColor(0x663d321d); c.drawRoundRect(track, 8, 8, p);
            float t = Math.max(0, Math.min(1, (value - min) / (max - min))); p.setColor(0xccffe477); c.drawRoundRect(new RectF(track.left, track.top, track.left + track.width() * t, track.bottom), 8, 8, p);
            p.setTextAlign(Paint.Align.RIGHT); p.setTypeface(Typeface.DEFAULT); p.setTextSize(h * .026f); p.setColor(0xfff1dc82); c.drawText(show, w * .78f, y, p);
        }

        String pct(float v, float mn, float mx) { return Math.round((v - mn) / (mx - mn) * 100) + "%"; }

        void drawControlsEditor(Canvas c, int w, int h) {
            drawBackroomsBackdrop(c, w, h, .86f);
            p.setTextAlign(Paint.Align.CENTER); p.setTypeface(Typeface.DEFAULT_BOLD); p.setTextSize(h * .05f); p.setColor(0xffffe28a); c.drawText("CONTROL LAYOUT", w / 2f, h * .12f, p);
            p.setStyle(Paint.Style.STROKE); p.setStrokeWidth(1); p.setColor(0x22fff0a0); for (int i = 1; i < 8; i++) c.drawLine(w * i / 8f, h * .16f, w * i / 8f, h * .86f, p); for (int i = 2; i < 8; i++) c.drawLine(w * .06f, h * i / 8f, w * .94f, h * i / 8f, p); p.setStyle(Paint.Style.FILL);
            p.setColor(0x22202010); c.drawRoundRect(new RectF(w * .45f, h * .22f, w * .86f, h * .74f), 18, 18, p); p.setColor(0x66fff0a0); label(c, new RectF(w * .45f, h * .22f, w * .86f, h * .74f), "LOOK AREA", h * .032f);
            p.setColor(0x55fff0a0); c.drawCircle(w * .17f, h * .68f, h * .12f * controlScale, p); p.setColor(0x88fff0a0); c.drawCircle(w * .17f, h * .68f, h * .048f * controlScale, p); p.setColor(0xffffe28a); label(c, new RectF(w * .05f, h * .80f, w * .29f, h * .86f), "JOYSTICK", h * .025f);
            menuButton(c, new RectF(w * .38f, h * .76f, w * .52f, h * .84f), "USE", false); menuButton(c, new RectF(w * .73f, h * .70f, w * .90f, h * .82f), "SPRINT", false); menuButton(c, new RectF(w * .86f, h * .06f, w * .94f, h * .15f), "Ⅱ", false);
            slider(c, w, h, 0, "Opacity", controlOpacity, .35f, 1f, pct(controlOpacity, .35f, 1f)); slider(c, w, h, 1, "Size", controlScale, .75f, 1.25f, pct(controlScale, .75f, 1.25f));
            menuButton(c, controlsReset(w,h), "RESET", false); menuButton(c, controlsBack(w,h), "BACK", false);
            p.setTextAlign(Paint.Align.LEFT);
        }

        void drawWin(Canvas c, int w, int h) {
            p.setColor(0xe0000000); c.drawRect(0, 0, w, h, p); p.setTextAlign(Paint.Align.CENTER); p.setTypeface(Typeface.DEFAULT_BOLD); p.setColor(0xffffe28a); p.setTextSize(h * .06f); c.drawText("YOU FOUND THE DOOR", w / 2f, h * .43f, p); p.setTextSize(h * .032f); p.setTypeface(Typeface.DEFAULT); c.drawText("Level 0 fades behind you.", w / 2f, h * .50f, p); menuButton(c, new RectF(w * .39f, h * .58f, w * .61f, h * .67f), "MAIN MENU", false); p.setTextAlign(Paint.Align.LEFT);
        }

        void menuButton(Canvas c, RectF r, String s, boolean disabled) {
            p.setStyle(Paint.Style.FILL); p.setColor(disabled ? 0x33333322 : 0x663d321d); c.drawRoundRect(r, 14, 14, p); p.setStyle(Paint.Style.STROKE); p.setStrokeWidth(2); p.setColor(disabled ? 0x55a09060 : 0xaaf1dc82); c.drawRoundRect(r, 14, 14, p); p.setStyle(Paint.Style.FILL); label(c, r, s, getHeight() * .031f);
        }

        void label(Canvas c, RectF r, String s, float size) { p.setTextAlign(Paint.Align.CENTER); p.setTypeface(Typeface.DEFAULT_BOLD); p.setTextSize(size); p.setColor(0xffffe28a); c.drawText(s, r.centerX(), r.centerY() + size * .36f, p); }

        @Override public boolean onTouchEvent(MotionEvent e) {
            int ac = e.getActionMasked(), idx = e.getActionIndex(), w = getWidth(), h = getHeight();
            if (ac == MotionEvent.ACTION_DOWN || ac == MotionEvent.ACTION_POINTER_DOWN) return down(e.getX(idx), e.getY(idx), e.getPointerId(idx), w, h);
            if (ac == MotionEvent.ACTION_MOVE) { move(e, w, h); return true; }
            if (ac == MotionEvent.ACTION_UP || ac == MotionEvent.ACTION_POINTER_UP || ac == MotionEvent.ACTION_CANCEL) return up(e.getPointerId(idx));
            return true;
        }

        boolean down(float X, float Y, int id, int w, int h) {
            if (screen == MENU) { if (menuEnter(w,h).contains(X,Y)) { newRun(); load = 0; screen = LOADING; } else if (menuSettings(w,h).contains(X,Y)) { previousScreen = MENU; screen = SETTINGS; } else if (menuExit(w,h).contains(X,Y)) ((Activity)getContext()).finish(); return true; }
            if (screen == LOADING) return true;
            if (screen == WIN) { if (new RectF(w * .39f, h * .58f, w * .61f, h * .67f).contains(X,Y)) screen = MENU; return true; }
            if (screen == SETTINGS) { settingsTap(X, Y, w, h); return true; }
            if (screen == CONTROLS) { controlsTap(X, Y, w, h); return true; }
            if (screen == PAUSE) { pauseTap(X, Y, w, h); return true; }
            if (pauseR(w,h).contains(X,Y)) { screen = PAUSE; return true; }
            if (useR(w,h).contains(X,Y)) { up = id; usePulse = true; return true; }
            if (sprintR(w,h).contains(X,Y)) { sp = id; sprint = true; return true; }
            if (X < w * .45f && lp < 0) { lp = id; joyx = jx = X; joyy = jy = Y; } else if (rp < 0) rp = id;
            return true;
        }

        void move(MotionEvent e, int w, int h) {
            for (int i = 0; i < e.getPointerCount(); i++) {
                int id = e.getPointerId(i); float X = e.getX(i), Y = e.getY(i);
                if (id == lp) { float rad = h * .13f * controlScale, dx = X - joyx, dy = Y - joyy, l = (float)Math.hypot(dx, dy); if (l > rad) { dx *= rad / l; dy *= rad / l; } jx = joyx + dx; jy = joyy + dy; mx = dx / rad; my = -dy / rad; }
                else if (id == rp && e.getHistorySize() > 0) ld += X - e.getHistoricalX(i, e.getHistorySize() - 1);
            }
        }

        boolean up(int id) { if (id == lp) { lp = -1; mx = my = 0; jx = joyx; jy = joyy; } if (id == rp) rp = -1; if (id == sp) { sp = -1; sprint = false; } if (id == up) up = -1; return true; }

        void pauseTap(float X, float Y, int w, int h) { if (pauseResume(w,h).contains(X,Y)) screen = PLAY; else if (pauseSettings(w,h).contains(X,Y)) { previousScreen = PAUSE; screen = SETTINGS; } else if (pauseControls(w,h).contains(X,Y)) { previousScreen = PAUSE; screen = CONTROLS; } else if (pauseExit(w,h).contains(X,Y)) screen = MENU; }

        void settingsTap(float X, float Y, int w, int h) {
            if (settingsBack(w,h).contains(X,Y)) { screen = previousScreen; return; }
            if (settingsControls(w,h).contains(X,Y)) { previousScreen = SETTINGS; screen = CONTROLS; return; }
            if (settingsFrame(w,h).contains(X,Y)) { frameCap = frameCap == 30 ? 45 : frameCap == 45 ? 60 : 30; save(); return; }
            for (int row = 0; row < 4; row++) {
                float y = h * (.265f + row * .095f);
                if (Y > y - h * .05f && Y < y + h * .035f && X > w * .40f && X < w * .75f) {
                    float t = Math.max(0, Math.min(1, (X - w * .43f) / (w * .29f)));
                    if (row == 0) sen = .5f + 1.9f * t; else if (row == 1) brightness = .65f + .7f * t; else if (row == 2) volume = t; else graphicsScale = .7f + .5f * t;
                    save(); return;
                }
            }
        }

        void controlsTap(float X, float Y, int w, int h) {
            if (controlsBack(w,h).contains(X,Y)) { screen = previousScreen; return; }
            if (controlsReset(w,h).contains(X,Y)) { controlOpacity = .78f; controlScale = 1; save(); return; }
            for (int row = 0; row < 2; row++) {
                float y = h * (.265f + row * .095f);
                if (Y > y - h * .05f && Y < y + h * .035f && X > w * .40f && X < w * .75f) {
                    float t = Math.max(0, Math.min(1, (X - w * .43f) / (w * .29f)));
                    if (row == 0) controlOpacity = .35f + .65f * t; else controlScale = .75f + .5f * t;
                    save(); return;
                }
            }
        }

        void back() { if (screen == PLAY) screen = PAUSE; else if (screen == PAUSE) screen = PLAY; else if (screen == SETTINGS || screen == CONTROLS) screen = previousScreen; else if (screen == WIN) screen = MENU; }

        RectF menuEnter(int w,int h){ return new RectF(w*.36f,h*.42f,w*.64f,h*.51f); } RectF menuSettings(int w,int h){ return new RectF(w*.36f,h*.55f,w*.64f,h*.64f); } RectF menuExit(int w,int h){ return new RectF(w*.36f,h*.68f,w*.64f,h*.77f); }
        RectF pauseResume(int w,int h){ return new RectF(w*.34f,h*.34f,w*.66f,h*.43f); } RectF pauseSettings(int w,int h){ return new RectF(w*.34f,h*.47f,w*.66f,h*.56f); } RectF pauseControls(int w,int h){ return new RectF(w*.34f,h*.60f,w*.66f,h*.69f); } RectF pauseExit(int w,int h){ return new RectF(w*.34f,h*.73f,w*.66f,h*.82f); }
        RectF settingsFrame(int w,int h){ return new RectF(w*.23f,h*.65f,w*.43f,h*.73f); } RectF settingsControls(int w,int h){ return new RectF(w*.45f,h*.65f,w*.67f,h*.73f); } RectF settingsBack(int w,int h){ return new RectF(w*.69f,h*.65f,w*.78f,h*.73f); }
        RectF controlsReset(int w,int h){ return new RectF(w*.34f,h*.86f,w*.48f,h*.94f); } RectF controlsBack(int w,int h){ return new RectF(w*.52f,h*.86f,w*.66f,h*.94f); }
        RectF sprintR(int w,int h){ float rr=h*.09f*controlScale; return new RectF(w-rr*2.6f,h-rr*2.35f,w-rr*.55f,h-rr*.3f); } RectF pauseR(int w,int h){ float s=h*.095f*controlScale; return new RectF(w-s*1.35f,h*.045f,w-s*.35f,h*.045f+s); } RectF useR(int w,int h){ float rw=h*.105f*controlScale; return new RectF(w*.50f-rw,h-rw*1.9f,w*.50f+rw,h-rw*.72f); }
        int mul(int color, float m) { return Color.argb(Color.alpha(color), cl((int)(Color.red(color)*m)), cl((int)(Color.green(color)*m)), cl((int)(Color.blue(color)*m))); }
        int lerp(int A, int B, float t) { return (int)(A * (1 - t) + B * t); } int cl(int v) { return Math.max(0, Math.min(255, v)); } float norm(float v) { while (v < -Math.PI) v += Math.PI * 2; while (v > Math.PI) v -= Math.PI * 2; return v; }
        static class Ray { float d, hx, hy; boolean side, light; } static class Spr { float x, y, d; int t; Spr(float X, float Y, int T, float D) { x = X; y = Y; t = T; d = D; } }
    }

    static class Sound {
        Thread t; boolean run; Game g;
        Sound(Game gg) { g = gg; }
        void start() {
            if (run) return;
            run = true;
            t = new Thread(() -> {
                int sr = 44100, bs = 2048;
                AudioTrack at = new AudioTrack.Builder()
                        .setAudioAttributes(new AudioAttributes.Builder().setUsage(AudioAttributes.USAGE_GAME).build())
                        .setAudioFormat(new AudioFormat.Builder().setSampleRate(sr).setChannelMask(AudioFormat.CHANNEL_OUT_STEREO).setEncoding(AudioFormat.ENCODING_PCM_16BIT).build())
                        .setBufferSizeInBytes(bs * 4).setTransferMode(AudioTrack.MODE_STREAM).build();
                short[] o = new short[bs * 2]; double z = 0, fp = 0, th = 0, nx = 3; Random r = new Random(); at.play();
                while (run) {
                    for (int i = 0; i < bs; i++) {
                        z += 1.0 / sr; double n = r.nextDouble() * 2 - 1; if (z > nx) { th = 1; nx = z + 4 + r.nextDouble() * 8; } th *= .9995;
                        double m = Math.min(1, Math.abs(g.mx) + Math.abs(g.my)); fp = (fp + m * (g.sprint ? .018 : .011)) % 1;
                        double foot = fp < .06 ? m * .12 * Math.sin(z * 430) : 0, hum = .05 * Math.sin(z * 377) + .025 * Math.sin(z * 754), buzz = .012 * n, mon = g.hunt ? (.065 * Math.sin(z * 42) + .02 * n) : 0;
                        double snd = (hum + buzz + th * .16 * Math.sin(z * 95) + foot + mon) * g.volume;
                        short v = (short)(Math.max(-1, Math.min(1, snd)) * 32767); o[i * 2] = v; o[i * 2 + 1] = (short)(v * .94);
                    }
                    at.write(o, 0, o.length, AudioTrack.WRITE_BLOCKING);
                }
                at.stop(); at.release();
            });
            t.start();
        }
        void stop() { run = false; }
    }
}
