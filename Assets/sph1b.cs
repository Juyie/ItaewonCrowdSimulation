// Unity3D C# (unfinished) version by mgear : http://unitycoder.com/blog
// NOTE! You need to enable "gizmos" in the play window, otherwise you dont see anything (lines are drawn with debug.drawline)

// see original copyright & license below
// original java source download: http://www.jamesdedge.com/SPHApplet.zip
// comments about it: http://stackoverflow.com/questions/5578891/smoothed-particle-hydrodynamics-fluid-sim

using UnityEngine;
using System.Collections;

public class sph1b : MonoBehaviour 
{

/*
 * Copyright 2011 James D. Edge
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

// import java.applet.*;
// import java.awt.*;
// import java.awt.event.*;
// import java.awt.image.*;
// import java.util.*;
// import java.io.*;
// import javax.imageio.*;

// class SPHAppletTimerTask extends TimerTask
// {
// 	
// 	private SPHApplet sph;
// 	
// 	public SPHAppletTimerTask(SPHApplet s)
// 	{ sph = s; }
// 	
// 	public void run()
// 	{ sph.step(); }
// 	
// }

// public class SPHApplet extends Applet implements KeyListener, MouseListener
// {
	
	public Color mycolor = new Color(0.0F, 0.0F, 0.0F, 1.0F);

	private int width;
	private int height;
	private int num_particles;
	
	private float poly6_const;
	private float poly6_grad_const;
	private float poly6_lap_const;
	private float spiky_grad_const;
	private float visc_lap_const;
	private float dt;
	private float dt2;
	private float cs;
	private float cs2;
	private float dampf;
	private float rest_density;
	private float viscosity;
	private float mass;
	private float imass;
	private float radius;
	private float iradius;
	private float radius2;
	private float iradius2;
	private float radius3;
	private float iradius3;
	
	private float[][] position = new float[4][];
	private float[][] prev_position = new float[4][];
	
	private float[][] velocity = new float[0][];
	private float[][] normals = new float[0][];
	private float[][] force = new float[0][];
	
	private float[] pressure = new float[0];
	private float[] density = new float[0];
	private float[] tension = new float[0];
	private float[] idensity = new float[0];
	private float[] idensity2 = new float[0];

	private float[][] plane_coord = new float[1][];
	private float[][] plane_norm = new float[0][];
	private float[][] distances = new float[0][];

//	private var timer:float;
//	private var BufferedImage buffer;
	private float[][] scalar_field = new float[0][];

	void Awake()
	{
		width = 100; //getSize().width;
		height = 100; //getSize().height;
		
		//setSize(width, height);
		
//		buffer = new BufferedImage(width, height, BufferedImage.TYPE_INT_ARGB);
	    scalar_field = new float[width/2+1][];
//	   scalar_field[0] = new float[width/2];
//	   scalar_field[1] = new float[height/2];
		for(int i = 0; i < width/2+1; ++i) 
			scalar_field[i] = new float[height/2+1];

		initSystem();
		
//		addKeyListener(this);
//		addMouseListener(this);
		
//		timer = new Timer();
//		timer.schedule(new SPHAppletTimerTask(this), 0, 60);		
	} // void start
	
	public float poly6(float r2)
	{
		return poly6_const*Mathf.Pow(radius2 - r2, 3);
	}
	
	public float poly6_grad(float r2)
	{ return poly6_grad_const*Mathf.Pow(radius2 - r2, 2); }
	
	public float poly6_lap(float r2)
	{ return poly6_lap_const*((radius2 - r2)*(7*r2 - 3*radius2)); }
	
	public float spiky_grad(float r)
	{ return spiky_grad_const*Mathf.Pow(radius - r, 2); }
	
	public float visc_lap(float r)
	{ return visc_lap_const*(radius - r); }
	
	void initSystem()
	{
		int dim = 40;
		
		num_particles = dim*dim;
		//print (num_particles);
		
		float orig_x = -0.25f;
		float orig_y = -0.25f;
		float w = 0.5f;
		float h = 0.5f;
		float off = 0.5f/(dim - 1);
		float area = w*h; // not used
		
		dt = 0.01f; dt2 = dt*dt;
		cs = 10.0f; cs2 = cs*cs;
		dampf = 0.15f;
		radius = 5*off; iradius = 1.0f/radius;
		radius2 = radius*radius; iradius2 = 1.0f/radius2;
		radius3 = radius*radius2; iradius3 = 1.0f/radius3;
		mass = 1.0f; imass = 1.0f/mass;
		rest_density = 25000.0f;
		viscosity = 1000.0f;
		
		poly6_const = 315.0f/(64.0f*Mathf.PI*Mathf.Pow(radius, 9));
		poly6_grad_const = -945.0f/(32.0f*Mathf.PI*Mathf.Pow(radius, 9));
		poly6_lap_const = 945.0f/(32.0f*Mathf.PI*Mathf.Pow(radius, 9));
		spiky_grad_const = -45.0f/(Mathf.PI*Mathf.Pow(radius, 6));
		visc_lap_const = 45.0f/(Mathf.PI*Mathf.Pow(radius, 6));
		
		position = new float[num_particles][]; //2
		
		for(int i = 0; i < num_particles; ++i) 
			position[i] = new float[2];
		
		prev_position = new float[num_particles][];
//		prev_position[0] = new float[2];
		
		for(int i = 0; i < num_particles; ++i) 
			prev_position[i] = new float[2];
		
		velocity = new float[num_particles][];
//		velocity[0] = new float[2];
		for(int i = 0; i < num_particles; ++i) 
			velocity[i] = new float[2];
		
		normals = new float[num_particles][];
//		normals[0] = new float[2];
		for(int i = 0; i < num_particles; ++i) 
			normals[i] = new float[2];

		force = new float[num_particles][];
		for(int i = 0; i < num_particles; ++i) 
			force[i] = new float[2];
//		force[0] = new float[2];
		
		pressure = new float[num_particles];
		tension = new float[num_particles];
		density = new float[num_particles];
		idensity = new float[num_particles];
		idensity2 = new float[num_particles];
		
		distances = new float[num_particles][];
//		distances[0] = new float[num_particles];
		for(int i = 0; i < num_particles; ++i) 
			distances[i] = new float[num_particles];


		plane_coord = new float[4][];
		plane_coord[0] = new float[2];
		plane_coord[1] = new float[2];
		plane_coord[2] = new float[2];
		plane_coord[3] = new float[2];
		
		plane_norm = new float[4][];
		plane_norm[0] = new float[2];
		plane_norm[1] = new float[2];
		plane_norm[2] = new float[2];
		plane_norm[3] = new float[2];
		
		//print (plane_coord.GetLength(1));
		
		plane_coord[0][0] = 0.0f; plane_coord[0][1] = -0.9f;
		plane_norm[0][0] = 0.0f; plane_norm[0][1] = 1.0f;
		plane_coord[1][0] = 0.9f; plane_coord[1][1] = 0.0f;
		plane_norm[1][0] = -1.0f; plane_norm[1][1] = 0.0f;
		plane_coord[2][0] = -0.9f; plane_coord[2][1] = 0.0f;
		plane_norm[2][0] = 1.0f; plane_norm[2][1] = 0.0f;
		plane_coord[3][0] = 0.0f; plane_coord[3][1] = 0.9f;
		plane_norm[3][0] = 0.0f; plane_norm[3][1] = -1.0f;
		

		for(int i = 0, j, k = 0; i < dim; ++i)
			for(j = 0; j < dim; ++j, ++k)
			{
			  position[k][0] = orig_x + i*off - 0.2f; position[k][1] = orig_y + j*off;
			  prev_position[k][0] = position[k][0];
				prev_position[k][1] = position[k][1];
				velocity[k][0] = -25.0f; velocity[k][1] = 0.0f;
				pressure[k] = 0.0f;
				density[k] = 0.0f;
			}
	} // void initsystem
	
	static float dx, dy, ddx, ddy, fx, fy, r, r2, den, Pij, Vij, Tij;
	
	public void computeDensity()
	{
		int i,j;
		
		for(i = 0; i < num_particles; ++i)
		  density[i] = 0.0f;
		
		for(i = 0; i < num_particles; ++i)
		{
			density[i] += mass*poly6(0.0f);
			
		  for(j = i + 1; j < num_particles; ++j)
			{
			  dx = (position[j][0] - position[i][0]);
				dy = (position[j][1] - position[i][1]);
				r2 = (dx*dx + dy*dy);
				if(r2 < radius2)
				{
					den = mass*poly6(r2);
					density[i] += den;
					density[j] += den;
//				distances[i][j] = (float)Mathf.Sqrt(r2);
//				print (j);
				distances[i][j] = Mathf.Sqrt(r2);
				}
				else
					distances[i][j] = -1.0f;
			}
		}
		
		for(i = 0; i < num_particles; ++i)
		{
			idensity[i] = 1.0f/density[i];
			idensity2[i] = idensity[i]*idensity[i];
		  pressure[i] = cs2*(density[i] - rest_density);
		}
	} //computeDensity
	
	void computeForces()
	{
		int i;
		int j;
		for(i = 0; i < num_particles; ++i)
		{
		  force[i][0] = -(dampf*velocity[i][0]);
			force[i][1] = -(9.8f + dampf*velocity[i][1]);
			tension[i] = 0.0f;
			normals[i][0] = 0.0f;
			normals[i][1] = 0.0f;
		}
		
		for(i = 0; i < num_particles; ++i)
		{
			for(j = i + 1; j < num_particles; ++j)
				if(distances[i][j] > 0.0f)
				{
				  r = distances[i][j];
					r2 = r*r;
				  dx = (position[i][0] - position[j][0]);
					dy = (position[i][1] - position[j][1]);
					ddx = (velocity[j][0] - velocity[i][0]);
					ddy = (velocity[j][1] - velocity[i][1]);
					Pij = -mass*(pressure[i]*idensity2[i] +
											 pressure[j]*idensity2[j])*
					spiky_grad(r);
					Vij = (viscosity*mass*visc_lap(r))*(idensity[i]*idensity[j]);
					
					fx = (Pij*dx + Vij*ddx);
					fy = (Pij*dy + Vij*ddy);
					
					force[i][0] += fx; force[i][1] += fy;
					force[j][0] -= fx; force[j][1] -= fy;
				}
		}
	} //computeForces
	
//	function collide(plane_coord:Array, plane_nrm:Array, ray_coord:Array, ray_dir:Array,isec:Array):float
	public float collide(float[] plane_coord, float[] plane_nrm,float[] ray_coord, float[] ray_dir,float[] isec)
	{
	  float dt = (plane_nrm[0]*ray_dir[0] + plane_nrm[1]*ray_dir[1]),
		dx = (ray_coord[0] - plane_coord[0]),
		dy = (ray_coord[1] - plane_coord[1]),
		dist = -(plane_nrm[0]*dx + plane_nrm[1]*dy)/dt;
		
		if(dist < 0.0) return -1.0f;
		
		isec[0] = ray_coord[0] + dist*ray_dir[0];
  		isec[1] = ray_coord[1] + dist*ray_dir[1];
		
		return dist;
	} //collide
	
	static float acc_x, acc_y, dist, ratio, ilen, step,nrm_dx, nrm_dy, tan_dx, tan_dy, dot;
	static float[] ray_coord = new float[2];
	static float[] ray_dir = new float[2];
	static float[] isec = new float[2];
	
	void integrate()
	{
		int i,j;
		
		for(i = 0; i < num_particles; ++i)
		{
			prev_position[i][0] = position[i][0];
      prev_position[i][1] = position[i][1];
      acc_x = force[i][0]*imass;
      acc_y = force[i][1]*imass;
      position[i][0] += (velocity[i][0]*dt + acc_x*dt2);
      position[i][1] += (velocity[i][1]*dt + acc_y*dt2);
      velocity[i][0] = (position[i][0] - prev_position[i][0])/dt;
			velocity[i][1] = (position[i][1] - prev_position[i][1])/dt;
			
			ray_coord[0] = prev_position[i][0];
		  ray_coord[1] = prev_position[i][1];
			ray_dir[0] = (position[i][0] - prev_position[i][0]);
			ray_dir[1] = (position[i][1] - prev_position[i][1]);
			
			ilen = 1.0f/Mathf.Sqrt(ray_dir[0]*ray_dir[0] + ray_dir[1]*ray_dir[1]);
			
			ray_dir[0] *= ilen;
			ray_dir[1] *= ilen;
			
			float prev_delt = dt;
			float delt = 0.0f;
			
			for(;;)
			{
			  for(j = 0; j < plane_coord.Length; ++j)
			  {
				  dist = collide(plane_coord[j], plane_norm[j], ray_coord, ray_dir, isec);
					
				  if(dist >= 0.0f)
			    {
			      ratio = (1.0f - (dist*ilen));
					  step = dt*ratio;
						
	  				if(step > delt)
	  				{
	  					delt = step;
							
		  				position[i][0] = isec[0] - 1.0e-4f*ray_dir[0];
		  				position[i][1] = isec[1] - 1.0e-4f*ray_dir[1];
							
		  				dot = (velocity[i][0]*plane_norm[j][0] + 
		  							 velocity[i][1]*plane_norm[j][1]);
							
	  					nrm_dx = plane_norm[j][0]*dot;
	  					nrm_dy = plane_norm[j][1]*dot;
	  					tan_dx = (velocity[i][0] - nrm_dx);
	 	  				tan_dy = (velocity[i][1] - nrm_dy);
							
		  				velocity[i][0] = (tan_dx - 0.6f*nrm_dx);
		  				velocity[i][1] = (tan_dy - 0.6f*nrm_dy);
		  			}
		  	  }
		  	}
				
		  	if(delt > 0.0f && delt < prev_delt)
		  	{
  			  prev_position[i][0] = position[i][0];
	  			prev_position[i][1] = position[i][1];
	  			position[i][0] += velocity[i][0]*delt;
	  			position[i][1] += velocity[i][1]*delt;
	  			prev_delt = delt;
	  		}
	  		else break;
		  }
			
			if(position[i][0] > 0.9f) position[i][0] = 0.9f;
			if(position[i][0] < -0.9f)	position[i][0] = -0.9f;
			if(position[i][1] < -0.9f) position[i][1] = -0.9f;
			if(position[i][1] > 0.9f) position[i][1] = 0.9f;
		}
	} // integrate

	public void computeScalarField()
	{
		for(int i = 0; i < scalar_field.Length; ++i)
			for(int j = 0; j < scalar_field[i].Length; ++j)
				scalar_field[i][j] = 0.0f;
		
	  for(int i = 0; i < num_particles; ++i)
		{
		  float x = (((float)(width/2))*((position[i][0] + 1.0f)/2)),
			y = (((float)(height/2))*(1.0f - (position[i][1] + 1.0f)/2)),
			xoff, yoff;
			
		  for(xoff = -5; xoff <= 5; ++xoff)
				for(yoff = -5; yoff <= 5; ++yoff)
				  if((xoff*xoff + yoff*yoff) < 16.0f)
					{
					  int ix = (int)Mathf.Round(x + xoff),
						iy = (int)Mathf.Round(y + yoff);
						float val = (16.0f - (xoff*xoff + yoff*yoff))/16.0f;
						
//					Debug.Log(scalar_field.Length + ";" + iy);
						if(val > scalar_field[ix][iy])
							scalar_field[ix][iy] = val;
					}
		}
	} // computeScalarField


	// main lopp **********************************
	void Update()
	{
		computeDensity();
		computeForces();
		integrate();
		computeScalarField();
		
//		Graphics g = buffer.getGraphics();
//		g.clearRect(0, 0, width, height);
//		g.setColor(Color.blue);
		
		print (num_particles);
		for(int i = 0; i < num_particles; ++i)
		{
			int x = (int)(width*((position[i][0] + 1.0f)/2)),
			y = (int)(height*(1.0f - (position[i][1] + 1.0f)/2));
			
			// setColor = Sets this graphics context's current color to the specified color. All subsequent graphics operations using this graphics context use this specified color. 
//			if(density[i] > (3*rest_density)) g.setColor(Color.red);
//			else if(density[i] > (2.5*rest_density)) g.setColor(Color.yellow);
//			else if(density[i] > (2*rest_density)) g.setColor(Color.green);
//			else if(density[i] > (1.5*rest_density)) g.setColor(Color.cyan);
//			else g.setColor(Color.blue);
			//mycolor = new Color(0.0F, 0.0F, 0.0F, 1.0F);
			
			if(density[i] > (3*rest_density)) mycolor = Color.red;
			else if(density[i] > (2.5*rest_density)) mycolor = Color.yellow;
			else if(density[i] > (2*rest_density)) mycolor = Color.green;
			else if(density[i] > (1.5*rest_density)) mycolor = Color.cyan;
			else mycolor = Color.blue;
			
//			g.fillRect(x - 1, y - 1, 2, 2);
			// actual points
			Debug.DrawLine(new Vector3(x-1, y-1,0), new Vector3(x-1, y,0),mycolor);
		} // for

//		print (mycolor);

		
//		g.setColor(Color.white);
		mycolor = new Color(1.0F, 1.0F, 1.0F, 1.0F);
	
		int off = 2;
		int half_off = 1;
		// Multiple Initializers and Incrementers : http://www.cafeaulait.org/course/week2/32.html 

		for(int i = 0, x = 0; i < (scalar_field.Length - 1); ++i, x += off)
		{
			for(int j = 0, y = 0; j < (scalar_field.Length - 1); ++j, y += off)
			{
				int idx = 0;
			
				if(scalar_field[i][j] > 0.1f) idx = (idx|0x000f);
				if(scalar_field[i][j + 1] > 0.1f) idx = (idx|0x00f0);
				if(scalar_field[i + 1][j + 1] > 0.1f) idx = (idx|0x0f00);
				if(scalar_field[i + 1][j] > 0.1f) idx = (idx|0xf000);


							
				if(idx == 0x0000 || idx == 0xffff) continue;
				else if(idx == 0xfff0 || idx == 0x000f)// || 
					Debug.DrawLine(new Vector3(x + half_off, y,0), new Vector3(x, y + half_off,0),mycolor);
				else if(idx == 0xff0f || idx == 0x00f0)// || 
//					g.drawLine(x, y + half_off, x + half_off, y + off);
					Debug.DrawLine(new Vector3(x, y + half_off,0), new Vector3(x + half_off, y + off,0),mycolor);
				else if(idx == 0xf0ff || idx == 0x0f00)// || 
//					g.drawLine(x + half_off, y + off, x + off, y + half_off);
					Debug.DrawLine(new Vector3(x + half_off, y+off,0), new Vector3(x + off, y + half_off,0),mycolor);
				else if(idx == 0x0fff || idx == 0xf000)
//					g.drawLine(x + off, y + half_off, x + half_off, y);
					Debug.DrawLine(new Vector3(x + off, y + half_off,0), new Vector3(x + half_off, y,0),mycolor);
				else if(idx == 0x00ff || idx == 0xff00)
//					g.drawLine(x + half_off, y, x + half_off, y + off);
					Debug.DrawLine(new Vector3(x + half_off, y,0), new Vector3(x + half_off, y + off,0),mycolor);
				else if(idx == 0xf00f || idx == 0x0ff0)
//					g.drawLine(x, y + half_off, x + off, y + half_off);
					Debug.DrawLine(new Vector3(x, y + half_off,0), new Vector3(x + off, y + half_off,0),mycolor);
				else if(idx == 0x0f0f)
				{
//					g.drawLine(x + half_off, y, x, y + half_off);
//					g.drawLine(x + half_off, y + off, x + off, y + half_off);
					Debug.DrawLine(new Vector3(x + half_off, y,0), new Vector3(x, y + half_off,0),mycolor);
					Debug.DrawLine(new Vector3(x + half_off, y + off,0), new Vector3(x + off, y + half_off,0),mycolor);
				}
				else if(idx == 0xf0f0)
				{
//					g.drawLine(x, y + half_off, x + half_off, y + off);
//					g.drawLine(x + off, y + half_off, x + half_off, y);
					Debug.DrawLine(new Vector3(x, y + half_off,0), new Vector3(x + half_off, y + off,0),mycolor);
					Debug.DrawLine(new Vector3(x + off, y + half_off,0), new Vector3(x + half_off, y,0),mycolor);
				}
			}
		} // for
//		mypaint();
	} // update
	
//	function paint(Graphics g)
//	public void mypaint()
//	{
//		if(g == null) return;
//		g.clearRect(0, 0, width, height);
//		g.setColor(Color.blue);
//		g.drawImage(buffer, 0, 0, null);
//		Debug.DrawLine(new Vector3(x, y + half_off,0), new Vector3(x + half_off, y + off,0),mycolor);
//	}
	
//	public void keyPressed(KeyEvent e) { step(); }
//	public void keyReleased(KeyEvent e) {}
//	public void keyTyped(KeyEvent e) {}
//	public void mouseClicked(MouseEvent e) {}
//	public void mouseEntered(MouseEvent e) {}
//	public void mouseExited(MouseEvent e) {}

//	float x, y;
/*
	public void mousePressed(MouseEvent e)
	{
		x = ((float)e.getX())/width;
		y = ((float)(height - e.getY()))/height;
		
		x = 2*x - 1.0f;
		y = 2*y - 1.0f;
	}
*/

/*
	public void mouseReleased(MouseEvent e)
	{
	  float nx = ((float)e.getX())/width,
		ny = ((float)(height - e.getY()))/height;
		
		nx = (2*nx - 1.0f) - x;
		ny = (2*ny - 1.0f) - y;
		
		for(int i = 0; i < num_particles; ++i)
		{
		  float dx = (position[i][0] - x),
			dy = (position[i][1] - y);
			
		  if((dx*dx + dy*dy) < 0.05f)
			{
			  velocity[i][0] += 15.0f*nx;
				velocity[i][1] += 15.0f*ny;
			}
		}
	}
*/

}